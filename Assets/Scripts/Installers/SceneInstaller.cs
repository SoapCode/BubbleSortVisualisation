using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Zenject;
using System.Linq;

namespace Algorithms
{	
	public class SceneInstaller : MonoInstaller
	{	
		public Settings SceneSettings;

		public override void InstallBindings()
		{
            InstallControllers();
			InstallSettings();
			InstallFactories ();
		}

        void InstallControllers()
        {
            Container.Bind<ITickable>().ToSingle<MainController>();
            Container.Bind<IInitializable>().ToSingle<MainController>();
            Container.Bind<MainController>().ToSingle();
        
			Container.BindAllInterfacesToSingle<BubbleSortController>();

			Container.Bind<ArrayHooks> ()
				.ToSinglePrefab<ArrayHooks> (SceneSettings.BSSettings.Array)
				.WhenInjectedInto<BubbleSortController>();

            Container.Bind<Camera>("Main").ToSingleInstance(SceneSettings.MainCamera);
            Container.Bind<CoroutinesManager>().ToSingleGameObject();
		}

		void InstallFactories()
		{
			//GO factories
            Container.BindGameObjectFactory<ColumnUnitHooks.Factory>(SceneSettings.BSSettings.Cube);
		}

		void InstallSettings()
		{
            Container.Bind<UIFunctions.Settings>().ToSingleInstance(SceneSettings.UISets.General);
            Container.Bind<BubbleSortController.Settings>().ToSingleInstance(SceneSettings.BSSettings.General);
		}

        #region Serializable classes-containers
        [Serializable]
        public class Settings
        {
            public Camera MainCamera;
            public UISettings UISets;
			public BubbleSortSettings BSSettings;

            [Serializable]
            public class UISettings
            {
                public UIFunctions.Settings General; 
            }

			[Serializable]
			public class BubbleSortSettings
			{
				public GameObject Array;
				public GameObject Cube;
				public BubbleSortController.Settings General;
			}
        }
        #endregion
	}
}
