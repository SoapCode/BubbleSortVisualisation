using UnityEngine;
using System.Collections;
using Zenject;

namespace Algorithms
{
	public class CameraControl : MonoBehaviour {

		public float zoomOffset;
		public float movementTime = 1f;

		Camera _thisCam;

        CoroutinesManager _crMng;

		[PostInject]
		void Init([Inject("Main")]Camera cam, CoroutinesManager crMng)
		{
			_thisCam = cam;
            _crMng = crMng;
		}

		public void ZoomToSize(float toX, float toY)
		{
			float fovTan = Mathf.Tan (Mathf.Deg2Rad * (_thisCam.fieldOfView / 2f));

			float horizFov = 2f * Mathf.Atan(fovTan * _thisCam.aspect);

			float distanceToCenter;

			if (toY > toX)
				distanceToCenter = (toY * 0.5f) / Mathf.Tan (Mathf.Deg2Rad * (_thisCam.fieldOfView / 2f));
			else
				distanceToCenter = (toX * 0.5f) / Mathf.Tan (horizFov / 2f);

			StartCoroutine (MoveSmoothly (new Vector3 (transform.position.x, toY / 2f, -distanceToCenter - zoomOffset)));
		}

		IEnumerator MoveSmoothly(Vector3 toPos)
		{
			float currentLerpTime = 0;
			_crMng.isRunning = true;
			while (this.transform.position != toPos) 
			{
				currentLerpTime += Time.deltaTime;
				if (currentLerpTime > movementTime)
				{
					currentLerpTime = movementTime;
				}

				float perc = currentLerpTime / movementTime;

				transform.position = Vector3.Lerp (this.transform.position, toPos, perc);
				yield return null;
			}
			_crMng.isRunning = false;
			if (EventManager.CameraMovementEnded != null)
				EventManager.CameraMovementEnded ();
		}

		/*void LateUpdate()
		{

		}*/




		// Camera target to look at.
		/*public Transform target;
		
		// Exposed vars for the camera position from the target.
		public float height = 20f;
		public float distance = 20f;
		
		// Camera limits.
		public float min = 10f;
		public float max = 60;
		
		// Rotation.
		public float rotateSpeed = 1f;
		
		// Options.
		public bool doRotate;
		public bool doZoom;
		
		// The movement amount when zooming.
		public float zoomStep = 30f;
		public float zoomSpeed = 5f;
		private float heightWanted;
		private float distanceWanted;
		
		// Result vectors.
		private Vector3 zoomResult;
		private Quaternion rotationResult;
		private Vector3 targetAdjustedPosition;
		
		void Start(){
			// Initialise default zoom vals.
			heightWanted = height;
			distanceWanted = distance;
			
			// Setup our default camera.  We set the zoom result to be our default position.
			zoomResult = new Vector3(0f, height, -distance);
		}
		
		void LateUpdate(){
			// Check target.
			if( !target ){
				Debug.LogError("This camera has no target, you need to assign a target in the inspector.");
				return;
			}
			
			if( doZoom ){
				// Record our mouse input.  If we zoom add this to our height and distance.
				float mouseInput = Input.GetAxis("Mouse ScrollWheel");
				heightWanted -= zoomStep * mouseInput;
				distanceWanted -= zoomStep * mouseInput;
				
				// Make sure they meet our min/max values.
				heightWanted = Mathf.Clamp(heightWanted, min, max);
				distanceWanted = Mathf.Clamp(distanceWanted, min, max);
				
				height = Mathf.Lerp(height, heightWanted, Time.deltaTime * zoomSpeed);
				distance = Mathf.Lerp(distance, distanceWanted, Time.deltaTime * zoomSpeed);
				
				// Post our result.
				zoomResult = new Vector3(0f, height, -distance);
			}
			
			if( doRotate ){
				// Work out the current and wanted rots.
				float currentRotationAngle = transform.eulerAngles.y;
				float wantedRotationAngle = target.eulerAngles.y;
				
				// Smooth the rotation.
				currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotateSpeed * Time.deltaTime);
				
				// Convert the angle into a rotation.
				rotationResult = Quaternion.Euler(0f, currentRotationAngle, 0f);
			}
			
			// Set the camera position reference.
			targetAdjustedPosition = rotationResult * zoomResult;
			transform.position = target.position + targetAdjustedPosition;
			
			// Face the desired position.
			transform.LookAt(target);
		}
*/
	}
}