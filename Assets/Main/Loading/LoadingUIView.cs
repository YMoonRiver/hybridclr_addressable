using TMPro;
using UnityEngine.UI;

namespace Wanderer.GameFramework
{
    public class LoadingUIView : UGUIView
    {
        public Slider sliderLoading;
        public TextMeshProUGUI txtTips;
        // Start is called before the first frame update
        public override void OnInit(IUIContext uiContext)
        {
            base.OnInit(uiContext);
            GameFrameworkMode.GetModule<EventManager>().AddListener<LoadingEventArgs>(OnLoading);

        }

        public override void OnFree(IUIContext uiContext)
        {
            base.OnFree(uiContext);
        }


        void OnLoading(object sender, IEventArgs e)
        {
            var args = (LoadingEventArgs)e;
            sliderLoading.value = args.Progress;
            txtTips.text = args.Tips;
        }
    }
}