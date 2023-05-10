using System;
using System.Linq;
using Sandbox;

namespace Woosh.Common;

public abstract class ObservableEntityComponent<T> : EntityComponent<T> where T : Entity, IObservableEntity
{
	[AttributeUsage( AttributeTargets.Method )]
	protected sealed class AutoAttribute : Attribute
	{
		public bool KeepAlive { get; set; }
	}

	private struct EventNode
	{
		public readonly Type Type;
		public readonly Delegate Delegate;

		public EventNode( Type type, Delegate evt )
		{
			Type = type;
			Delegate = evt;
		}
	}

	protected Dispatcher Events => Entity.Events;
	
	private EventNode[] m_Nodes;

	protected override void OnActivate()
	{
		// Auto Register Events from Attribute

		if ( m_Nodes != null )
		{
			foreach ( var node in m_Nodes )
			{
				Events.Inject( node.Type, node.Delegate );
			}

			return;
		}

		var methods = TypeLibrary.GetMethodsWithAttribute<AutoAttribute>().ToArray();
		m_Nodes = new EventNode[methods.Length];

		for ( var index = 0; index < methods.Length; index++ )
		{
			var pair = methods[index];

			var method = pair.Method;

			var type = TryFindArgument( method );
			var cb = new DynamicCallback( e => method.Invoke( this, new[] { e } ) );

			Events.Inject( type, cb );
			m_Nodes[index] = new EventNode( type, cb );
		}
	}

	private Type TryFindArgument( MethodDescription description )
	{
		var parameters = description.Parameters;

		if ( parameters.Length != 1 )
			return null;

		if ( parameters[0].ParameterType.GenericTypeArguments.Length != 1 )
			return null;

		return parameters[0].ParameterType.GenericTypeArguments[0];
	}

	protected override void OnDeactivate()
	{
		// Auto Unregister Events from Attribute
		foreach ( var node in m_Nodes )
		{
			Events.Erase( node.Type, node.Delegate );
		}
	}
}
