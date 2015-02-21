using UnityEngine;
using System.Collections;

public class Environment : Singleton<Environment> {

	enum Flag {
		Pause	= (1<<0),
	}

	Flag flags = 0;

	public void Pause() { flags |= Flag.Pause; }
	public void Resume() { flags &= ~Flag.Pause; }
	public bool isPause { get { return (flags & Flag.Pause) != 0; } }
}
