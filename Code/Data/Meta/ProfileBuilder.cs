using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Woosh.Data;

public struct ProfileBuilder
{
#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder(Profile from)
	{
		m_DisplayName = from.Display;
		m_Brief = from.Brief;
		m_Description = from.Description;
		m_Authors = new HashSet<Author>(from.Authors);
		m_Website = from.Website;
		m_License = from.License;
		m_Group = from.Group;
		m_Category = from.Category;
		m_Tags = new HashSet<string>(from.Tags);
		m_Version = from.Version;
		m_Size = from.Size;
		m_Thumbnail = from.Thumbnail;
		m_Schema = from.Binding;
		m_Statistics = from.Statistics;
	}

	private string m_DisplayName;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithDisplayName(string name)
	{
		m_DisplayName = name;
		return this;
	}

	private string m_Brief;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithBreif(string brief)
	{
		m_Brief = brief;
		return this;
	}

	private string m_Description;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithDescription(string description)
	{
		m_Description = description;
		return this;
	}

	private string m_Schema;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithSchema(string schema)
	{
		m_Schema = schema;
		return this;
	}

	private Statistics? m_Statistics;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithStatistics(Statistics stats)
	{
		m_Statistics = stats;
		return this;
	}

	private HashSet<Author> m_Authors;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithAuthor(Author author)
	{
		(m_Authors ??= new HashSet<Author>()).Add(author);
		return this;
	}

	private string m_Website;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder FromWebsite(string website)
	{
		m_Website = website;
		return this;
	}

	private string m_License;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithLicense(string license)
	{
		m_License = license;
		return this;
	}

	private string m_Group;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder FromGroup(string group)
	{
		m_Group = group;
		return this;
	}

	private string m_Category;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder FromCategory(string category)
	{
		m_Category = category;
		return this;
	}

	private HashSet<string> m_Tags;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithTag(string tag)
	{
		(m_Tags ??= new HashSet<string>()).Add(tag);
		return this;
	}

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithTags(params string[] tags)
	{
		(m_Tags ??= new HashSet<string>()).UnionWith(tags);
		return this;
	}

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithoutTag(string tag)
	{
		m_Tags.Add(tag);
		return this;
	}

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithoutTags(params string[] tags)
	{
		m_Tags?.ExceptWith(tags);
		return this;
	}

	private uint m_Version;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithVersion(uint version)
	{
		m_Version = version;
		return this;
	}

	private ulong m_Size;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithSize(ulong size)
	{
		m_Size = size;
		return this;
	}

	private string m_Thumbnail;

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public ProfileBuilder WithThumbnail(string thumbnail)
	{
		m_Thumbnail = thumbnail;
		return this;
	}

#if !DEVELOPMENT_BUILD || !DEBUG
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
	public static implicit operator Profile(ProfileBuilder builder)
	{
		var profile = new Profile(builder.m_DisplayName)
		{
			Tags = builder.m_Tags?.ToArray() ?? Array.Empty<string>(),
			Brief = builder.m_Brief,
			Description = builder.m_Description,
			Authors = builder.m_Authors?.ToArray() ?? Array.Empty<Author>(),
			Website = builder.m_Website,
			License = builder.m_License,
			Group = builder.m_Group,
			Category = builder.m_Category,
			Version = builder.m_Version,
			Size = builder.m_Version,
			Thumbnail = builder.m_Thumbnail,
			Binding = builder.m_Schema,
			Statistics = builder.m_Statistics,
		};

		return profile;
	}
}
