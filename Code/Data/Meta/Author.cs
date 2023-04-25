namespace Woosh.Data;

public readonly struct Author
{
	public string Name { get; }
	public string Brief { get; init; }
	public string Picture { get; init; }

	public static implicit operator Author(string name) => new Author(name);

	public Author(string name)
	{
		Name = name;
		Brief = null;
		Picture = null;

		Twitter = null;
		YouTube = null;
		Instagram = null;
		Website = null;
		Email = null;
	}

	public string Twitter { get; init; }
	public string YouTube { get; init; }
	public string Website { get; init; }
	public string Email { get; init; }
	public string Instagram { get; init; }
}
