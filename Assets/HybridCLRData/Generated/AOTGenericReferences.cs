public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//System.Action`1<System.Object>
	//System.Action`1<System.Int64>
	//System.Action`2<System.Object,System.Object>
	//System.Action`2<System.Object,System.Int32>
	//UnityEngine.Events.UnityAction`1<System.Byte>
	//UnityEngine.Events.UnityEvent`1<System.Byte>
	//Wanderer.GameFramework.FSM`1<System.Object>
	//Wanderer.GameFramework.FSMState`1<System.Object>
	//Wanderer.GameFramework.GameEventArgs`1<System.Object>
	// }}

	public void RefMethods()
	{
		// System.Object[] System.Array::Empty<System.Object>()
		// System.Object UniRx.DisposableExtensions::AddTo<System.Object>(System.Object,UnityEngine.Component)
		// System.IDisposable UniRx.ObservableExtensions::Subscribe<System.Int64>(System.IObservable`1<System.Int64>,System.Action`1<System.Int64>)
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.GameObject::AddComponent<System.Object>()
		// System.Void Wanderer.GameFramework.EventManager::AddListener<System.Object>(System.Action`2<System.Object,Wanderer.GameFramework.IEventArgs>)
		// System.Void Wanderer.GameFramework.EventManager::RemoveListener<System.Object>(System.Action`2<System.Object,Wanderer.GameFramework.IEventArgs>)
		// System.Void Wanderer.GameFramework.FSManager::AddFSM<System.Object>()
		// System.Object Wanderer.GameFramework.FSManager::GetFSM<System.Object>()
		// System.Void Wanderer.GameFramework.FSMState`1<System.Object>::ChangeState<System.Object>(Wanderer.GameFramework.FSM`1<System.Object>)
		// System.Object Wanderer.GameFramework.GameFrameworkMode::GetModule<System.Object>()
	}
}