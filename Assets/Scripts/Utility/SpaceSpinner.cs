using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using LetterBattle.Utility;
using LetterBattle.Utility.Utility;

public class SpaceSpinner : TweenSpawnerMono
{
	protected override Tween ConstructTween()
	{
		return	this.transform.DoRotateAboutZ(360, Duration).SetLoops(-1).SetEase(Ease.Linear).SetLink(this.gameObject);

	}
}
