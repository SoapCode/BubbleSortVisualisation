using UnityEngine;
using Zenject;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using System.Collections;

namespace Algorithms
{

    public class BubbleSortController : IInitializable {

		//Input
		string _clmnHgtText;
		List<int> _nmbrsInt;

		//Settings
		Settings _settings = null;

		//Components
		ArrayHooks _arr;
        CoroutinesManager _crMng;

		//GOs
		GameObject _sceneParent;
		GameObject _array;
        GameObject _mediator;

		//Properties
		public string ClmnHgt { get { return _clmnHgtText; }}

		//Factories
		ColumnUnitHooks.Factory _cbFactory;
		IInstantiator _gameObjectCreator;

		//Misc
		Camera _mainCam;

		public BubbleSortController(ColumnUnitHooks.Factory factory, ArrayHooks arr,
		                            Settings settings, [Inject ("Main")]Camera mainCam,
		                            IInstantiator gameObjectCreator,
                                    CoroutinesManager crMng)
		{
			_cbFactory = factory;
			_settings = settings;
			_arr = arr;
			_mainCam = mainCam;
			_gameObjectCreator = gameObjectCreator;
            _crMng = crMng;
		}

        //Handlers--------------------------------------------
		bool GenButtonHandler(string clmnHgt)
		{
			_clmnHgtText = clmnHgt;
			ParseInput ();

			float size = _nmbrsInt.Count;
			if (size != 0) {
				float bndX = size + size * _settings.Gap;
				float bndY = _nmbrsInt.Max ();
				CameraControl cc = _mainCam.GetComponent<CameraControl>();
				cc.ZoomToSize(bndX,bndY);
			}
			return GenerateColumns ();

		}

        void RetryButtonHandler()
        {
            foreach (Transform clmn in _array.transform)
            {
                foreach (Transform cube in clmn)
                {
                    GameObject.Destroy(cube.gameObject);
                }
                GameObject.Destroy(clmn.gameObject);
            }
            _mediator.transform.DOMoveY(5f, 1f).
                SetRelative().
                OnComplete(()=> GameObject.Destroy(_mediator));
        }
        //-----------------------------------------------------

		//TODO: make parsing better, maybe use regular expressions
		void ParseInput()
		{
			_nmbrsInt = new List<int>();

			if (_clmnHgtText != "")
			{
				string[] nmbrsStr = _clmnHgtText.Split (',');

				for (int i = 0; i < nmbrsStr.Length; i++)
					_nmbrsInt.Add ( Int32.Parse (nmbrsStr [i]));
			} 
			else
			{
				int size = UnityEngine.Random.Range (5, 20);

				for (int i = 0; i < size; i++)
					_nmbrsInt.Add( UnityEngine.Random.Range (1, 20));
			}
		}

		bool GenerateColumns()
		{
			if (_nmbrsInt.Count != 0)
			{
				SpawnColumns();
				return true;
			} 
			else
			{
				return false;
			}
		}

		void SpawnColumns()
		{

			_array = _arr.gameObject;

			float size = _nmbrsInt.Count;
			size += size * _settings.Gap;
			float hlfSize = size / 2;
			float step = size / _nmbrsInt.Count;

			string clmName = "Column";

			float cnt = 0;

			foreach (int nmb in _nmbrsInt) 
			{
				GameObject column = new GameObject(clmName);
				column.transform.SetParent(_array.transform);
				column.transform.localPosition = new Vector3(
					cnt - hlfSize,0,0 );

				for(int j = 0;j < nmb;j++)
				{
					GameObject cube = _cbFactory.Create().gameObject;
					Transform cubeTran = cube.transform;
					cubeTran.SetParent(column.transform);
					cubeTran.position = new Vector3(cubeTran.parent.localPosition.x,j,0);
				}
				cnt += step;
			}
		}

		void BubbleSortAnimation()
		{
			_mediator = _gameObjectCreator.InstantiatePrefab (_settings.Mediator);
			_mediator.transform.SetParent (_sceneParent.transform);
			_mediator.transform.position = new Vector3 (-_nmbrsInt.Count, _nmbrsInt.Max() + 5f,0);

			List<GameObject> arrayOfGOs = new List<GameObject>();
			foreach (Transform cl in _array.transform)
			{
				arrayOfGOs.Add(cl.gameObject);
			}
            _mediator.transform.DOMoveY(_mediator.transform.position.y - 4.5f, 1f).
                OnComplete(()=>_crMng.StartCoroutine(InvokeSortingAnimation(arrayOfGOs)));
        }

        IEnumerator InvokeSortingAnimation(List<GameObject> arrayOfGOs)
        {
            _crMng.isRunning = true;

            int size = arrayOfGOs.Count;

            Renderer medRend = _mediator.GetComponent<Renderer>();

            for (int i = 0; i < size - 1; i++)
            {
                for (int j = 0; j < size - i - 1; j++)
                {

                    float centre = arrayOfGOs[j].transform.position.x + 1f;

                    Vector3 po = new Vector3(centre, _mediator.transform.position.y, 0);
                    Tween md = _mediator.transform.DOMove(po,1f).SetRecyclable(true);
                    yield return md.WaitForCompletion();

                    if (arrayOfGOs[j].transform.childCount > arrayOfGOs[j + 1].transform.childCount)
                    {
                        medRend.material.color = Color.red;

                        GameObject tempGO = arrayOfGOs[j].gameObject;

                        DOTween.To(
                            x => arrayOfGOs[j].transform.DOMove(
                                new Vector3(centre + Mathf.Cos(x) * 1f, 0, Mathf.Sin(x) * 1f),
                                0),
                            0, Mathf.PI, 1f).SetRecyclable(true);
                        Tweener mv2 = DOTween.To(
                            x => arrayOfGOs[j+1].transform.DOMove(
                                new Vector3(centre + Mathf.Cos(x) * 1f, 0, Mathf.Sin(x) * 1f),
                                0),
                             Mathf.PI, Mathf.PI * 2f, 1f).SetRecyclable(true);
                       
                        arrayOfGOs[j] = arrayOfGOs[j + 1];
                        arrayOfGOs[j + 1] = tempGO;

                        yield return mv2.WaitForCompletion();
                    }
                    medRend.material.color = Color.green;
                }
               
                foreach(Transform cube in arrayOfGOs[size-i-1].transform)
                {
                    cube.GetComponent<Renderer>().material.color = Color.green;
                }
            }
            //TODO: deal with this repetition or not?
            foreach (Transform cube in arrayOfGOs[0].transform)
            {
                cube.GetComponent<Renderer>().material.color = Color.green;
            }
            _crMng.isRunning = false;
            if (EventManager.SceneEnded != null)
                EventManager.SceneEnded();
            else
                Debug.Log("SceneEnded event is empty");
        }

        public void Initialize()
		{
			GUIEventManager.GenerateButtonEvent += GenButtonHandler;
			GUIEventManager.StartAnimEvent += BubbleSortAnimation;
            GUIEventManager.RetryBtnEvent += RetryButtonHandler;
			_sceneParent = _settings.SceneParent;
			_arr.transform.SetParent (_sceneParent.transform);
		}

		#region Settings
		[Serializable]
		public class Settings
		{
			public GameObject SceneParent;
			public GameObject Mediator;
			public float Gap;
		}

		#endregion

	}
}