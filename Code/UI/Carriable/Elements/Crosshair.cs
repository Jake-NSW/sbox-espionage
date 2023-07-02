using Sandbox.UI;
using Woosh.Signals;

namespace Woosh.Espionage.UI;

public sealed class Crosshair : Panel
{
	private int m_FireCount;

	public Crosshair()
	{
		StyleSheet.Load("/ui/Carriable/Elements/Crosshair.scss");
		
		for ( var i = 0; i < 5; i++ )
		{
			var p = Add.Panel( "element" );
			p.AddClass( $"el{i}" );
		}
	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "fire", m_FireCount > 0 );

		if ( m_FireCount > 0 )
			m_FireCount--;
	}

	public void Fired( Event<WeaponFired> evt )
	{
		m_FireCount += 2;
	}
}
