public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	// GameFramework.dll
	// UniRx.dll
	// UnityEngine.CoreModule.dll
	// mscorlib.dll
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Action<object>
	// System.Action<long>
	// System.Action<object,object>
	// System.Action<object,int>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityEvent<byte>
	// Wanderer.GameFramework.FSM<object>
	// Wanderer.GameFramework.FSMState<object>
	// Wanderer.GameFramework.GameEventArgs<object>
	// }}

	public void RefMethods()
	{
		// object[] System.Array.Empty<object>()
		// object UniRx.DisposableExtensions.AddTo<object>(object,UnityEngine.Component)
		// System.IDisposable UniRx.ObservableExtensions.Subscribe<long>(System.IObservable<long>,System.Action<long>)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// System.Void Wanderer.GameFramework.EventManager.AddListener<object>(System.Action<object,Wanderer.GameFramework.IEventArgs>)
		// System.Void Wanderer.GameFramework.EventManager.RemoveListener<object>(System.Action<object,Wanderer.GameFramework.IEventArgs>)
		// System.Void Wanderer.GameFramework.FSManager.AddFSM<object>()
		// object Wanderer.GameFramework.FSManager.GetFSM<object>()
		// System.Void Wanderer.GameFramework.FSMState<object>.ChangeState<object>(Wanderer.GameFramework.FSM<object>)
		// object Wanderer.GameFramework.GameFrameworkMode.GetModule<object>()
	}
}