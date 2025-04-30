using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_GameScene_CardBase<T> : UI_Base where T : CardBase
{
    #region Enum to Bind
    protected enum Buttons
    {
        CardButton,
    }

    protected enum Images
    {
        CardButton,
        CardImage,
    }
    #endregion

    public T Card { get; protected set; }

    ECardState _cardUIState = ECardState.None;
    public ECardState CardUIState
    {
        get { return _cardUIState; }
        protected set
        {
            if (_cardUIState == value) return;

            _cardUIState = value;

            switch (value)
            {
                case ECardState.Idle:
                    startOffset = Random.Range(0f, Mathf.PI * 2f);
                    break;
                case ECardState.Moving:
                    break;
            }
        }
    }

    protected CardManager _cardManager { get { return Managers.CardManager; } }

    public RectTransform RectTransform { get; protected set; }

    #region Init & SetInfo
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        tiltSettings = Managers.Resource.Load<IdleTiltSettings>("IdleTiltSettings");

        return true;
    }

    public virtual void SetInfo(T card)
    {
        transform.localScale = Vector3.one;

        Card = card;

        // Setting UI Image
        GetImage((int)Images.CardImage).sprite = Managers.Resource.Load<Sprite>(card.CardData.FrontSpriteName);

        // SetActive -> 1프레임 뒤에 Position Setting // GridLayGroup 초기화 이슈,
        gameObject.SetActive(true);
        StartCoroutine(CaptureOriginalNextFrame());

        // Init State
        CardUIState = card.CardState;
    }
    protected virtual void OnStateChanged(ECardState state)
    {
        CardUIState = state;
    }

    IEnumerator CaptureOriginalNextFrame()
    {
        yield return new WaitForEndOfFrame();
        Card.OriginalPosition = RectTransform.position;
    }
    #endregion

    #region Update
    private void Update()
    {
        OnUpdate();
    }

    private void OnUpdate()
    {
        switch (CardUIState)
        {
            case ECardState.Idle:
                UpdateIdle();
                break;
            case ECardState.Moving:
                UpdateMoving();
                break;
        }
    }
    
    protected virtual void UpdateIdle()
    {
        CardTilt();
    }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateAdd() { }
    protected virtual void UpdateRemove() { }
    #endregion

    #region Animation & Tweeing
    private float startOffset; // 시작 시점을 달리하여, 각 카드가 다른 Motion일 수 있도록 만들기 위함
    private IdleTiltSettings tiltSettings;
    private void CardTilt()
    {
        // 사인, 코사인 계산
        float time = Time.time + startOffset;
        float sinValue = Mathf.Sin(time);
        float cosValue = Mathf.Cos(time);

        // 현재 Euler 각도 읽기
        Vector3 euler = RectTransform.eulerAngles;

        // X, Y 축 각각 LerpAngle 보간
        float newX = Mathf.LerpAngle(euler.x, sinValue * tiltSettings.maxAngle, tiltSettings.lerpSpeed * Time.deltaTime);
        float newY = Mathf.LerpAngle(euler.y, cosValue * tiltSettings.maxAngle, tiltSettings.lerpSpeed * Time.deltaTime);

        // Apply
        RectTransform.eulerAngles = new Vector3(newX, newY, 0f);
    }
    #endregion

    public void UpdatePositionFromCard()
    {
        RectTransform.position = Card.OriginalPosition;
    }

    protected virtual bool TrySwap(PointerEventData evt, bool isBoth)
    {
        if (evt.pointerEnter == null)
            return false;

        return true;
    }

    #region Event Handle
    protected virtual void OnClick(PointerEventData evt)
    {
        if (HasMoved()) return;
        Card.CardState = ECardState.Idle; // To Do : Select State

        Debug.Log("On Click");
    }

    protected Vector3 _dragOffset;
    protected virtual void OnBeginDrag(PointerEventData evt)
    {
        _dragOffset = RectTransform.position - (Vector3)evt.position;
        Card.CardState = ECardState.Moving;

        Debug.Log("On Begin Drag");
    }

    protected virtual void OnDrag(PointerEventData evt)
    {
        Debug.Log($"On Drag");

        GetImage((int)Images.CardButton).raycastTarget = false;

        RectTransform.position = (Vector3)evt.position + _dragOffset;
    }

    protected virtual void OnEndDrag(PointerEventData evt)
    {
        Card.CardState = ECardState.Idle;
        RectTransform.position = Card.OriginalPosition;
        GetImage((int)Images.CardButton).raycastTarget = true;
    }
    #endregion

    #region Helper
    protected bool HasMoved()
    {
        return Card.OriginalPosition != RectTransform.position;
    }
    #endregion
}
