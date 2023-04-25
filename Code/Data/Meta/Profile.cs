using System;
using System.Runtime.CompilerServices;

namespace Woosh.Data;

public readonly struct Statistics
{
	public uint Downloads { get; init; }
	public uint Likes { get; init; }
	public uint Dislikes { get; init; }
	public uint Favourites { get; init; }
	public uint Supporters { get; init; }
}

/// <summary>
/// A Profile is a series of data that is used to define something in UI.
/// It comes with preset fields that define it as a profile.
/// </summary>
public readonly struct Profile
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ProfileBuilder Build() => new ProfileBuilder();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ProfileBuilder Build(string display) => new ProfileBuilder().WithDisplayName(display);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ProfileBuilder Build(Profile from) => new ProfileBuilder(from);

	public Profile(string displayName)
	{
		Display = displayName;
		Tags = Array.Empty<string>();

		Brief = null;
		Description = null;
		Binding = null;
		Authors = null;
		Statistics = null;
		Category = null;
		Group = null;

		Version = 0;
		Size = 0;

		Thumbnail = default;
		Website = null;
		License = null;
	}

	/// <summary>
	/// A UI friendly name that relates to the data this is attached too.
	/// Make sure its title-case!
	/// </summary>
	public string Display { get; }

	/// <summary>
	/// A short UI friendly descriptor that provides a incredibly brief phrase
	/// explaining what the data is or how it gets interacted wtih
	/// </summary>
	public string Brief { get; init; }

	/// <summary>
	/// A Description that represents what the data is. This is to be used in
	/// UI.
	/// </summary>
	public string Description { get; init; }

	/// <summary>
	/// The binding is the "file extension" of the attached data. Or how the
	/// data can be interacted with
	/// </summary>
	public string Binding { get; init; }

	/// <summary>
	/// The statistics that belong to the attached data. This is a generalization
	/// of what statistics would exist on the attached data.
	/// </summary>
	public Statistics? Statistics { get; init; }

	/// <summary>
	/// A UI friendly name that represents the author of the attached data.
	/// </summary>
	public Author[] Authors { get; init; }

	/// <summary>
	/// A Website that is associated with this Profile. Used in the UI to link
	/// to something. Usually used the Display text with this as a hyperlink.
	/// </summary>
	public string Website { get; init; }

	/// <summary>
	/// A License that represents the attached data. This is used in the UI
	/// to tell the user what the license of the attached object is.
	/// </summary>
	public string License { get; init; }

	/// <summary>
	/// A UI friendly named group that groups the data, that is used in a
	/// way that describes where or how this profile is used.
	/// </summary>
	public string Group { get; init; }

	/// <summary>
	/// A UI friendly name that represents the type of data this is and what
	/// category it should fall under.
	/// </summary>
	public string Category { get; init; }

	/// <summary>
	/// Tags are attached string based meta data that allows you to inject
	/// key only data.
	/// </summary>
	public string[] Tags { get; init; }

	/// <summary>
	/// The Version of the attached item. This usually is used for checking
	/// if an updated object is available
	/// </summary>
	public uint Version { get; init; }

	/// <summary>
	/// How large is the chunk of memory or file size is the data we're
	/// profiling, represented in bytes.
	/// </summary>
	public ulong Size { get; init; }

	/// <summary>
	/// A Icon of the attached data. Used in the UI to represent the
	/// object this Profile is for.
	/// </summary>
	public string Icon { get; init; }
	
	/// <summary>
	/// A Thumbnail of the attached data. Used in the UI to represent the
	/// object this Profile is for.
	/// </summary>
	public string Thumbnail { get; init; }
}
