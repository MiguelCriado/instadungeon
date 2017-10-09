using DG.Tweening;
using InstaDungeon.Components;
using InstaDungeon.Models;
using RSG;
using System;
using UnityEngine;
using UnityEngine.UI;

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

		private SpriteRenderer[] renderersList;
		private Color[] originalRenderersColor;
		private Vector3 replaceItemOriginalPosition;
		private CanvasGroup canvasGroup;
		private Entity entity;
		private Item replaceItem;
		private bool visible;
		private Guid sequenceId;

		private void Awake()
		{
			replaceItemOriginalPosition = replaceItemAvatar.transform.localPosition;
			canvasGroup = GetComponent<CanvasGroup>();
			sequenceId = Guid.NewGuid();

			renderersList = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
			originalRenderersColor = new Color[renderersList.Length];

			for (int i = 0; i < renderersList.Length; i++)
			{
				originalRenderersColor[i] = renderersList[i].color;
				Color color = renderersList[i].color;
				color.a = 0f;
				renderersList[i].color = color;
			}

			downwardsArrow.transform.Translate(Vector3.up * 0.125f);
			downwardsArrow.transform.DOLocalMoveY(downwardsArrow.transform.localPosition.y - 0.25f, 0.75f)
			.SetEase(Ease.OutSine).SetLoops(-1, LoopType.Restart);
		}

		private void OnEnable()
		{
			replaceItemAvatar.transform.localPosition = replaceItemOriginalPosition;
			downwardsArrow.gameObject.SetActive(true);
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

		public IPromise AnimateReplaceItem()
		{
			Promise result = new Promise();
			Vector3 finalLocation = replaceItemAvatar.transform.parent.InverseTransformPoint(currentItemAvatar.transform.position);

			downwardsArrow.DOPause();
			downwardsArrow.gameObject.SetActive(false);

			replaceItemAvatar.transform.DOLocalMoveY(finalLocation.y, 0.4f)
			.SetEase(Ease.InBack)
			.OnComplete(() => 
			{
				result.Resolve();
			});

			return result;
		}

		public IPromise Show()
		{
			Promise result = new Promise();

			if (visible == false)
			{
				DOTween.Kill(sequenceId);
				Sequence sequence = DOTween.Sequence()
				.SetId(sequenceId);

				for (int i = 0; i < renderersList.Length; i++)
				{
					sequence.Join(renderersList[i].DOFade(originalRenderersColor[i].a, 0.3f));
				}

				sequence.OnComplete(() => 
				{
					result.Resolve();
				});

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

			if (visible == true)
			{
				DOTween.Kill(sequenceId);
				Sequence sequence = DOTween.Sequence()
				.SetId(sequenceId);

				for (int i = 0; i < renderersList.Length; i++)
				{
					sequence.Join(renderersList[i].DOFade(0, 0.3f));
				}

				sequence.OnComplete(() =>
				{
					result.Resolve();
				});

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
