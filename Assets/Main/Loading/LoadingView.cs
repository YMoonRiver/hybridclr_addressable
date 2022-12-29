using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class LoadingView : MonoBehaviour
    {
        public Slider sliderLoading;
        public TextMeshProUGUI txtTips;
        // Start is called before the first frame update
        void Start()
        {
            GameFrameworkMode.GetModule<EventManager>().AddListener<LoadingEventArgs>(OnLoading);

        }



        void OnLoading(object sender, IEventArgs e)
        {
            var args = (LoadingEventArgs)e;
            sliderLoading.value = args.Progress;
            txtTips.text = args.Tips;
        }
    }
}