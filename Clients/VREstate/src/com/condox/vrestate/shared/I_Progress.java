package com.condox.vrestate.shared;

public interface I_Progress {

	public enum ProgressType {
		Error, Loading, Processing, Executing, Wait, None
	}

	public abstract void SetupProgress(ProgressType label);

	public abstract void UpdateProgress(double percent);

	public abstract void CleanupProgress();

}