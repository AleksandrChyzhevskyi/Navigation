using BLINK.RPGBuilder.LogicMono;
using UnityEngine;

namespace _Development.Scripts.Navigation
{
    public class ModelInfinity : MonoBehaviour
    {
        [SerializeField] private MovementPathHelperPoints _helperPoints;

        private void OnEnable() => 
            CombatEvents.PlayerDeathAccepted += ActiveViewElements;

        private void OnDisable() => 
            CombatEvents.PlayerDeathAccepted += ActiveViewElements;

        public void On()
        {
            ActiveViewElements();
            _helperPoints.PlayerNavigation.SetOnNavigation();
            _helperPoints.PlayerNavigation.CheckReactivateAgent(true);
        }

        public void Off()
        {
            RPGBuilderEssentials.Instance.Minimap.gameObject.SetActive(false);
            RPGBuilderEssentials.Instance.QuestsController.QuestsContent.gameObject.SetActive(false);
            _helperPoints.PlayerNavigation.SetOffNavigation();
        }

        private void ActiveViewElements()
        {
            if(RPGBuilderEssentials.Instance.Minimap.gameObject.activeInHierarchy)
                return;
            
            RPGBuilderEssentials.Instance.Minimap.gameObject.SetActive(true);
            RPGBuilderEssentials.Instance.QuestsController.QuestsContent.gameObject.SetActive(true);
            RPGBuilderEssentials.Instance.QuestsController.Timer.gameObject.SetActive(false);
        }
    }
}