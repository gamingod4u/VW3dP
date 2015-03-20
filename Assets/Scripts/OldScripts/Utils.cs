using UnityEngine;
using System.Collections;

public static class Utils {

	public static Vector3 RotateOrigPosition(Vector3 source, Vector3 center, Vector3 axis, float angle) {
		Vector3 pos = source;
		Quaternion rot = Quaternion.AngleAxis(angle, axis); // get the desired rotation
		Vector3 dir = pos - center; // find current direction relative to center
		dir = rot * dir; // rotate the direction
		pos = center + dir; // define new position
		
		return pos;
	}

	public static Vector3 DrawCircle ( int idx, Vector3 center, float radius){
		float ang = (((float)idx / 15f) * 360);
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		pos.y = center.y;
		pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
		return pos;
	}
}
