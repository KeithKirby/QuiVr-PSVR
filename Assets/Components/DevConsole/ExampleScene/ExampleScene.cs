using UnityEngine;

namespace DevConsole
{
    public class ExampleScene : MonoBehaviour
    {
        private bool rotating = false;
        private Vector3 currentRotation;
        private Vector3 target = new Vector3(180, 240, 180);

        public void ToggleRotation()
        {
            rotating = !rotating;
        }

        void Start()
        {
            DevConsole.Instance.AddCommand("dyn_example", "An example dynamically added function", ExampleFunc);
            DevConsole.Instance.AddCommand("dyn_echo", "An example dynamic function with a parameter", "[str]", DynamicEcho);
            DevConsole.Instance.AddCommand("dyn_params", "An example dynamic function with params[]", "[str, ...]", DevConsole.CreateDynamicCommandDelegate(DynamicParamsFunction));
            currentRotation = gameObject.transform.eulerAngles;
        }

        void FixedUpdate()
        {
            if (!rotating) return;
            if (currentRotation == target) currentRotation = Vector3.zero;
            currentRotation = Vector3.Lerp(currentRotation, target, Time.deltaTime * 0.05f);
            gameObject.transform.eulerAngles = currentRotation;
        }

        public static string ExampleFunc()
        {
            return "ExampleFunc() called!";
        }
        public static string DynamicEcho(string s)
        {
            return s;
        }

        public static string DynamicParamsFunction(params string[] args)
        {
            return string.Join(" ", args);
        }
    }
}