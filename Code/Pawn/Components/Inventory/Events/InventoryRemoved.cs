﻿using Sandbox;
using Woosh.Signals;

namespace Woosh.Espionage;

public readonly record struct InventoryRemoved( Entity Item ) : ISignal;
