using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Sandbox;

namespace Woosh.Common;

public abstract class ObservableEntityComponent<T, TComponents> : ObservableEntityComponent<T> where T : Entity, IObservableEntity where TComponents : struct, ITuple
{
	protected override void OnActivate()
	{
		base.OnActivate();

		if ( Game.IsServer )
			_ = Components;
	}
	
	public TComponents Components
	{
		get
		{
			Log.Info(TypeLibrary.GetType<TComponents>());
			var types = TypeLibrary.GetType<TComponents>().GenericArguments;
			Log.Info(types);
			
			var args = new object[types.Length];

			for ( int i = 0; i < args.Length; i++ )
			{
				args[i] = Entity.Components.GetOrCreateAny( types[i] );
			}

			var value = TypeLibrary.Create<TComponents>( typeof(TComponents), args );
			return value;
		}
	}
}

public abstract class ObservableEntityComponent<T> : EntityComponent<T>
	where T : Entity, IObservableEntity
{
	[AttributeUsage( AttributeTargets.Method )]
	protected sealed class AutoAttribute : Attribute
	{
		public Type Override { get; set; }
	}

	private struct EventNode
	{
		public readonly Type Type;
		public readonly Action<object, object> Delegate;

		public EventNode( Type type, Action<object, object> evt )
		{
			Type = type;
			Delegate = evt;
		}
	}

	private readonly static EventNode[] m_Nodes;

	static ObservableEntityComponent()
	{
		/*
		var methods = TypeLibrary.GetMethodsWithAttribute<AutoAttribute>().ToArray();
		m_Nodes = new EventNode[methods.Length];

		for ( var index = 0; index < methods.Length; index++ )
		{
			var (method, attribute) = methods[index];

			var type = TryFindArgument( method );
			var cb = new Action<object, object>( ( target, evt ) => method.Invoke( target, new[] { evt } ) );

			m_Nodes[index] = new EventNode( attribute.Override ?? type, cb );
		}
		*/
	}

	private static Type TryFindArgument( MethodDescription description )
	{
		var parameters = description.Parameters;

		// if ( parameters.Length != 1 )
		return null;

		/*
		if ( parameters[0].ParameterType.GenericTypeArguments.Length != 1 )
			return null;

		return parameters[0].ParameterType.GenericTypeArguments[0];
		*/
	}

	protected Dispatcher Events => Entity.Events;

	protected override void OnActivate()
	{
		/*
		if ( m_Nodes == null )
		{
			return;
		}

		foreach ( var node in m_Nodes )
		{
			Events.Inject( node.Type, new DynamicCallback( obj => node.Delegate.Invoke( this, obj ) ) );
		}
		*/
	}

	protected override void OnDeactivate()
	{
		/*
		// Auto Unregister Events from Attribute
		foreach ( var node in m_Nodes )
		{
			// Events.Erase( node.Type, (DynamicCallback)Invoke );
		}
		*/
	}
}
