using Sandbox;

namespace Woosh.Espionage;

public abstract class FirearmMagazine : ModelEntity, IFirearmAmmoProvider
{
	protected FirearmMagazine( int capacity )
	{
		m_Ammo = new int[capacity];
	}

	public int Spare => Capacity - m_Caret;
	public int Capacity => m_Ammo.Length;

	private int m_Caret;
	private readonly int[] m_Ammo;

	public bool Add( int? type = null )
	{
		if ( Spare == Capacity )
			return false;

		m_Caret++;
		return true;
	}

	public (bool Valid, int Type) Consume( int amount )
	{
		m_Caret--;
		
		return default;
	}
}
