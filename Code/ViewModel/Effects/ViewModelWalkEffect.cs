﻿using Woosh.Common;
using Woosh.Signals;

namespace Woosh.Espionage;

public sealed class ViewModelWalkEffect : ObservableEntityComponent<CompositedViewModel>, IViewModelEffect
{
	public void OnPostCameraSetup( ref CameraSetup setup )
	{
		
	}
}