using InstaDungeon.Components;
using InstaDungeon.Models;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RSG;

namespace InstaDungeon
{
	[RequireComponent(typeof(CanvasGroup))]
	public class ItemInteractionController : MonoBehaviour
	{
		public Item ReplaceItem { get { return replaceItem; } }

		[Header("References")]
		[SerializeField] private SpriteRenderer currentItemAvatar;
		[SerializeField] private SpriteRenderer replaceItemAvatar;
		[SerializeField] private SpriteRenderer downwardsArrow;
		[Space]
		[SerializeField] private Text currentItemHint;
		[SerializeField] private Text replaceItemHint;

		private CanvasGroup canvasGroup;
		private Entity entity;
		private Item replaceItem;
		private bool visible;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			downwardsArrow.transform.Translate(Vector3.up * 0.125f);

			downwardsArrow.transform.DOLocalMoveY(downwardsArrow.transform.localPosition.y - 0.25f, 0.75f)
			.SetEase(Ease.OutSine).SetLoops(-1, LoopType.Restart);
		}

		private void OnEnable()
		{
			downwardsArrow.DOPlay();
		}

		private void OnDisable()
		{
			downwardsArrow.DOPause();

			entity = null;
			replaceItem = null;

			currentItemAvatar.sprite = null;
			replaceItemAvatar.sprite = null;

			currentItemHint.text = "";
			replaceItemHint.text = "";
		}

		public void Initialize(Entity entity, Item replaceItem)
		{
			this.entity = entity;
			this.replaceItem = replaceItem;

			Inventory inventory = entity.GetComponent<Inventory>();
			Item currentItem = inventory.GetItem(replaceItem.ItemInfo.InventorySlot);

			if (currentItem != null)
			{
				currentItemAvatar.sprite = currentItem.ItemInfo.Avatar;
				replaceItemHint.text = "";
			}
			
			replaceItemAvatar.sprite = replaceItem.ItemInfo.Avatar;
			currentItemHint.text = "";

			Show();

			// TODO subscribe to inventory events to animate this cacharro
		}

		public IPromise Show()
		{
			Promise result = new Promise();

			if (visible == false)
			{


				visible = true;
			}
			else
			{
				result.Resolve();
			}

			return result;
		}

		public IPromise Hide()
		{
			Promise result = new Promise();

			if (visible == false)
			{
				visible = false;
			}
			else
			{
				result.Resolve();
			}

			return result;
		}

		public void Dispose()
		{
			Hide().Done(() => gameObject.Recycle());
		}
	}
}
