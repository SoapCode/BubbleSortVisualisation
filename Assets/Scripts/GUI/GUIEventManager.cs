using System;

namespace Algorithms
{

    public static class GUIEventManager {

		public static Action StartButtonAction = delegate {};
		public static Action QuitButtonAction = delegate {};
		public static Action StartAnimEvent = delegate {};
        public static Action RetryBtnEvent = delegate { };

        public delegate bool GenerateButtonHandler(string input);
		public static GenerateButtonHandler GenerateButtonEvent;

	}
}