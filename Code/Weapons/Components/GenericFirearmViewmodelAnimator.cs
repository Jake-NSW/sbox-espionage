using Woosh.Common;

namespace Woosh.Espionage;

public sealed class GenericFirearmViewmodelAnimator : ObservableEntityComponent<Firearm>
{
	protected override void OnActivate()
	{
		base.OnActivate();
		Events.Register<WeaponFireEvent>( () => Entity.SetAnimParameter( "bFire", true ) );
	}
}
