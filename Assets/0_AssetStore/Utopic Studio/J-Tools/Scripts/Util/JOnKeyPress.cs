using UnityEngine;

namespace J
{

	[AddComponentMenu("J/Util/JOnKeyPress")]
	public class JOnKeyPress : JBase
    {

        [SerializeField] string inputName = "Submit";

		[TooltipAttribute("Maximum seconds between the two taps")]
		[SerializeField]	float pressCooldown = 0.2f;

		[Range(1, 50)]
		[SerializeField]	int pressCount = 1;
		[SerializeField]	CooldownType cooldownType;
        [SerializeField] UnityEngine.Events.UnityEvent onInput;

        private bool isDoingCooldown = false;
		private float pressCooldownPrivate;
		private int pressCountPrivate;
		private float deltaTimeUsed;



        void Start () {
			Init ();
		}

		void Init () {
			pressCountPrivate = 0;
			pressCooldownPrivate = pressCooldown;
			isDoingCooldown = false;
		}
		void ResetCooldown () {
			pressCooldownPrivate = pressCooldown;
		}

		void Update () {

			if (Input.GetButtonDown(inputName)) {
				ResetCooldown ();
				if (pressCount > 1)
					isDoingCooldown = true;
				pressCountPrivate++;
				if (pressCountPrivate >= pressCount) {
					CallActions ();
					Init ();
				}
			}

			if (isDoingCooldown) {
				ReduceCooldown ();


				if (pressCooldownPrivate <= 0f) {
					Init ();
				}
			}
		}

		void CallActions() {
			onInput.Invoke ();
		}

		void ReduceCooldown() {
			switch (cooldownType) {
			case CooldownType.RealSpeed:
				deltaTimeUsed = Time.unscaledDeltaTime;
				break;
			case CooldownType.GameSpeed:
				deltaTimeUsed = Time.deltaTime;
				break;
			default:
				break;
			}
			pressCooldownPrivate -= deltaTimeUsed;
		}
	}


	enum CooldownType
	{
		RealSpeed, GameSpeed
	}
}