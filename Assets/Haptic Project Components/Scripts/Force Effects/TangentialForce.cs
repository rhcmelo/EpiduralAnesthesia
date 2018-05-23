﻿using UnityEngine;
using System.Collections;

public class TangentialForce : MonoBehaviour {

	public string Type;
	public int effect_index;
	public float gain;
	public float magnitude;
	public float duration;
	public float frequency;
	public float[] positionEffect =  new float[3];
	public float[] directionEffect = new float[3];

	// Use this for initialization
	void Start () {
		Type = "tangentialForce";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
