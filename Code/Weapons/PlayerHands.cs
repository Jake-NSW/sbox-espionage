using Sandbox;
using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

[Library, Title( "Hands" ), Icon( "pan_tool" )]
public sealed partial class PlayerHands : MeleeWeapon, ISlotted<CarrySlot>, IHave<EntityInfo>, INetworkSerializer
{
	public PlayerHands()
	{
		Events.Register<CreatedViewModel>(
			evt =>
			{
				var view = evt.Data.ViewModel;
				view.Model = Model.Load( "weapons/hands/v_espionage_hands.vmdl" );
				view.Components.Create<GenericFirearmViewModelAnimator>();
				view.Build().WithAspect( new ViewModelEffectsAspect() ).WithoutAnyComponent<ViewModelPitchOffsetEffect>();
			}
		);
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		if ( Game.IsServer && Input.Pressed( "shoot" ) )
		{
			Log.Info( "penis" );
			using ( var writer = NetWrite.StartRpc2( 951950591 ) )
			{
				writer.Write( "Hello World!" );
				writer.SendRpc2( To.Single( Owner ), this );
			}
		}
	}

	protected override void OnCallRemoteProcedure( int id, NetRead read )
	{
		Log.Info( "recieved rpc" );
		if ( id == 951950591 )
		{
			Log.Info( read.ReadString() );
		}

		base.OnCallRemoteProcedure( id, read );
	}

	public override void Spawn()
	{
		Transmit = TransmitType.Owner;
	}

	public CarrySlot Slot => CarrySlot.Utility;
	public EntityInfo Item { get; } = new EntityInfo() { Nickname = "Hands", Display = "Hands" };
	
	public void Read( ref NetRead read ) { }
	public void Write( NetWrite write ) { }
}
