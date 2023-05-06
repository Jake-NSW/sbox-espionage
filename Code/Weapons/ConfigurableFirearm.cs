﻿using Sandbox;

namespace Woosh.Espionage;

[GameResource( "Espionage Weapon Asset", "weapon", "Used for describing a Weapon item type in Espionage" )]
public sealed class WeaponDataAsset : GameResource
{
	// Meta Data
	
	[Category( "Info" )] public string Display { get; set; }
	
	// Visuals

	[ResourceType( "vmdl" ), Category( "Visuals" )]
	public string WorldModel { get; set; }

	[ResourceType( "vmdl" ), Category( "Visuals" )]
	public string ViewModel { get; set; }
	
	// Gameplay

	public DrawTime Draw { get; set; }
}

public sealed partial class ConfigurableFirearm : Firearm, IHave<DisplayInfo>
{
	public WeaponDataAsset Asset { get => n_Asset; init => OnAssetChanged( value ); }

	// Meta

	public DisplayInfo Item => new DisplayInfo() { Name = Asset.Display };

	// Asset

	[Net] private WeaponDataAsset n_Asset { get; set; }

	private void OnAssetChanged( WeaponDataAsset asset )
	{
		n_Asset = asset;
		Model = Model.Load( Asset.WorldModel );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	// Weapon

	public override DrawTime Draw => Asset.Draw;

	protected override AnimatedEntity OnRequestViewmodel()
	{
		var view = new CompositedViewModel( Events ) { Owner = Owner, Model = Model.Load( Asset.ViewModel ) };
		view.Add( new ViewModelOffsetEffect( Vector3.Zero, default ) );
		view.Add( new ViewModelSwayEffect( 1, 1.3f ) );
		view.Add( new ViewModelMoveOffsetEffect( Vector3.One, 10 ) );
		view.Add( new ViewModelStrafeOffsetEffect() { Damping = 6, RollMultiplier = 1, AxisMultiplier = 10 } );
		view.Add( new ViewModelDeadzoneSwayEffect( new Vector2( 10, 10 ) ) { AimingOnly = true, AutoCenter = false, Damping = 8 } );
		view.Add( new ViewModelPitchOffsetEffect( 5, 4 ) { Damping = 15 } );
		view.Add( new ViewModelRecoilEffect() );
		view.Add( new ViewModelTuckEffect() );

		view.SetBodyGroup( "module", 1 );
		// view.SetBodyGroup( "muzzle", 1 );

		return view;
	}
}