public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//System.Action`1<System.Object>
	//System.Action`2<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2<System.Object,System.Object>
	//System.Collections.Generic.IEnumerator`1<System.Object>
	//System.Collections.Generic.List`1<System.Object>
	//System.Collections.Generic.List`1<UnityEngine.Vector2>
	//UnityEngine.Events.UnityAction`2<UnityEngine.SceneManagement.Scene,UnityEngine.SceneManagement.LoadSceneMode>
	//Wanderer.GameFramework.FSM`1<System.Object>
	//Wanderer.GameFramework.FSMState`1<System.Object>
	//Wanderer.GameFramework.GameEventArgs`1<System.Object>
	// }}

	public void RefMethods()
	{
		// System.Object[] System.Array::Empty<System.Object>()
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.GameObject::AddComponent<System.Object>()
		// System.Object UnityEngine.GameObject::GetComponent<System.Object>()
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object)
		// System.Void Wanderer.GameFramework.EventManager::AddListener<System.Object>(System.Action`2<System.Object,Wanderer.GameFramework.IEventArgs>)
		// System.Void Wanderer.GameFramework.EventManager::RemoveListener<System.Object>(System.Action`2<System.Object,Wanderer.GameFramework.IEventArgs>)
		// System.Void Wanderer.GameFramework.FSMState`1<System.Object>::ChangeState<System.Object>(Wanderer.GameFramework.FSM`1<System.Object>)
		// System.Object Wanderer.GameFramework.GameFrameworkMode::GetModule<System.Object>()
	}
}