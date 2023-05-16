using System;
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
			Log.Info( TypeLibrary.GetType<TComponents>() );
			var types = TypeLibrary.GetType<TComponents>().GenericArguments;
			Log.Info( types );

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

public abstract class ObservableEntityComponent<T> : EntityComponent<T> where T : Entity, IObservableEntity
{
	protected Dispatcher Events => Entity.Events;
}
