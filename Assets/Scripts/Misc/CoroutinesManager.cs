using UnityEngine;
using System.Collections;

namespace Algorithms
{

	public class CoroutinesManager : MonoBehaviour {

		public bool isRunning = false;

        public delegate void Task();

        public void Invoke(Task task, float time)
        {
            Invoke(task.Method.Name, time);
        }
    }
}