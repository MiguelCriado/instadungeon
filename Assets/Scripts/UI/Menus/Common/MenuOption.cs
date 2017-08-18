using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InstaDungeon.UI
{
	public class MenuOption : Selectable, ISubmitHandler
	{
		public UnityEvent OnOptionSelected = new UnityEvent();
		public UnityEvent OnOptionPressed = new UnityEvent();

		public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
		{
			base.OnSelect(eventData);

			if (OnOptionSelected != null)
			{
				OnOptionSelected.Invoke();
			}
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);

			Select();
		}

		public void OnSubmit(BaseEventData eventData)
		{
			SubmitOption();
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);

			SubmitOption();
		}

		private void SubmitOption()
		{
			if (OnOptionPressed != null)
			{
				OnOptionPressed.Invoke();
			}
		}
	}
}
