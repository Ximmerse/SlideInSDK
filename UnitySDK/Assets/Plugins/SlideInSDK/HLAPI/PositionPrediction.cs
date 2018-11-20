using UnityEngine;

namespace Ximmerse.SlideInSDK
{
	/// <summary>
	/// Position prediction
	/// </summary>
	internal class PositionPrediction
	{
		#region Public Properties

		/// <summary>
		/// If we should smooth the prediction due to a pop in tracking.
		/// </summary>
		/// <value><c>true</c> if smooth; otherwise, <c>false</c>.</value>
		public bool Smooth
		{
			get
			{
				return smooth;
			}
			set
			{
				smooth = value;

				// We need to add another frame due to the smoothing
				maxFrameCount = (smooth) ? 8 : 6;
				minFrameCount = (smooth) ? 4 : 3;
			}
		}

		/// <summary>
		/// The speed used when smoothing, lower the faster.
		/// </summary>
		public float SmoothSpeed { get; set; }

		#endregion

		#region Private Properties

		// For 60 FPS
		private int maxFrameCount = 8; // 6 without smooth
		private const int MaxFrameRate = 60;

		// For 30 FPS
		private int minFrameCount = 4; // 3 without smooth
		private const int MinFrameRate = 30;

		private bool smooth;

		private int frameCount = 5;
		private int pastFrameIndex;
		private int framesPassed;
		private float deltaTime;
		private int framesPerSecond;

		private Vector3[] lastPositions = new Vector3[8];
		private float[] lastCompassCorrections = new float[8];
		private Quaternion[] lastXRotations = new Quaternion[8];
		private Quaternion[] lastYRotations = new Quaternion[8];
		private Quaternion[] lastZRotations = new Quaternion[8];
		private Vector3 velocity = Vector3.zero;

		// For prediction based on rotation
		private Quaternion change;
		private Vector3 changeAngles;
		private float compassAdjust;

		#endregion

		#region Public Methods

		public PositionPrediction(bool useSmoothing = false)
		{
			SmoothSpeed = 0.02f;
			Smooth = useSmoothing;
		}

		/// <summary>
		/// Gets the position with prediction.
		/// </summary>
		/// <returns>The prediction.</returns>
		/// <param name="position">Position.</param>
		public Vector3 GetPrediction(Vector3 position)
		{
			UpdateFrameRate();

			int currentFrameIndex = pastFrameIndex;
			int adjustedFrameIndex = Mod(currentFrameIndex - frameCount, maxFrameCount);

			// Save for next time
			lastPositions[currentFrameIndex] = position;
			pastFrameIndex = (pastFrameIndex + 1 >= maxFrameCount) ? 0 : pastFrameIndex + 1;

			if (Smooth)
			{
				return Vector3.SmoothDamp(lastPositions[currentFrameIndex], lastPositions[currentFrameIndex] + (lastPositions[currentFrameIndex] - lastPositions[adjustedFrameIndex]), ref velocity, SmoothSpeed);
			}

			return lastPositions[currentFrameIndex] + (lastPositions[currentFrameIndex] - lastPositions[adjustedFrameIndex]);
		}

		/// <summary>
		/// Gets the prediction based on rotation.
		/// </summary>
		/// <returns>The prediction.</returns>
		/// <param name="currentRotation">Current rotation.</param>
		/// <param name="currentPosition">Current position.</param>
		/// <param name="compassCorrection">Compass correction.</param>
		public Vector3 GetPrediction(Quaternion currentRotation, Vector3 currentPosition, float compassCorrection)
		{
			UpdateFrameRate();

			int currentFrameIndex = pastFrameIndex;
			int adjustedFrameIndex = Mod(currentFrameIndex - frameCount, maxFrameCount);

			Vector3 holder = currentRotation.eulerAngles;
			holder.x = 0.0f;
			holder.z = 0.0f;
			lastYRotations[currentFrameIndex] = Quaternion.Euler(holder);

			holder = currentRotation.eulerAngles;
			holder.y = 0.0f;
			holder.z = 0.0f;
			lastXRotations[currentFrameIndex] = Quaternion.Euler(holder);

			holder = currentRotation.eulerAngles;
			holder.x = 0.0f;
			holder.y = 0.0f;
			lastZRotations[currentFrameIndex] = Quaternion.Euler(holder);

			pastFrameIndex = (pastFrameIndex + 1 >= maxFrameCount) ? 0 : pastFrameIndex + 1;

			change = (lastXRotations[adjustedFrameIndex] * Quaternion.Inverse(lastXRotations[currentFrameIndex]));
			currentPosition = change * currentPosition;

			change = (lastYRotations[adjustedFrameIndex] * Quaternion.Inverse(lastYRotations[currentFrameIndex]));
			compassAdjust = lastCompassCorrections[adjustedFrameIndex] - lastCompassCorrections[currentFrameIndex];
			changeAngles = change.eulerAngles;
			changeAngles.y -= compassAdjust;
			change = Quaternion.Euler(changeAngles);
			currentPosition = change * currentPosition;

			change = (lastZRotations[adjustedFrameIndex] * Quaternion.Inverse(lastZRotations[currentFrameIndex]));
			currentPosition = change * currentPosition;

			return currentPosition;
		}

		#endregion

		#region Private Methods

		private void UpdateFrameRate()
		{
			framesPassed++;
			deltaTime += Time.deltaTime;

			if (deltaTime > 1.0)
			{
				framesPerSecond = (int)(framesPassed / deltaTime);
				framesPassed = 0;
				deltaTime -= 1.0f;

				// +5 is so when we round we go up a bit when getting the frame count
				frameCount = (int)Mathf.Lerp(minFrameCount - 1, maxFrameCount - 1, Mathf.InverseLerp(MinFrameRate, MaxFrameRate, framesPerSecond + 5));
			}
		}

		private static int Mod(int value, int modulus)
		{
			int remainder = value % modulus;
			return remainder < 0 ? remainder + modulus : remainder;
		}

		#endregion
	}
}