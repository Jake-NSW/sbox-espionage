﻿namespace Woosh.Common;

public delegate void StructCallback<T>( in Event<T> evt ) where T : struct, IEventData;