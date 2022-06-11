using UnityEngine;

namespace DG.Tweening.Plugins.Options
{
	public struct PathOptions : IPlugOptions
	{
		public PathMode mode;

		public OrientType orientType;

		public AxisConstraint lockPositionAxis;

		public AxisConstraint lockRotationAxis;

		public bool isClosedPath;

		public Vector3 lookAtPosition;

		public Transform lookAtTransform;

		public float lookAhead;

		public bool hasCustomForwardDirection;

		public Quaternion forward;

		public bool useLocalPosition;

		public Transform parent;

		public bool isRigidbody;

		internal Quaternion startupRot;

		internal float startupZRot;

		public void Reset()
		{
			mode = PathMode.Ignore;
			orientType = OrientType.None;
			lockPositionAxis = (lockRotationAxis = AxisConstraint.None);
			isClosedPath = false;
			lookAtPosition = Vector3.zero;
			lookAtTransform = null;
			lookAhead = 0f;
			hasCustomForwardDirection = false;
			forward = Quaternion.identity;
			useLocalPosition = false;
			parent = null;
			startupRot = Quaternion.identity;
			startupZRot = 0f;
		}
	}
}
