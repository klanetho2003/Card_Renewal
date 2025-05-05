using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static Define;

public interface ICardAnimator
{
    void PlayIdle(bool isRayCast);
    void PlayHover();
    void PlayPointerDown();
    void PlaySelect();
    void PlayMoving();
    void ResetToIdle();
}

public class CardAnimator<TUI, T> : ICardAnimator where TUI : UI_GameScene_CardBase<T> where T : CardBase
{
    private readonly TUI _owner;

    private RectTransform _cardRect;
    private RectTransform _imageRect;
    private RectTransform _shadowRect;

    // ScriptableObject
    private IdleTiltSetting _tilt;
    private HoverSetting _hover;
    private ClickSetting _click;
    private MoveSetting _move;

    private Vector3 _originalShadowPos;

    public CardAnimator(TUI owner, RectTransform card, RectTransform image, RectTransform shadow,
        IdleTiltSetting tilt, HoverSetting hover, ClickSetting click, MoveSetting move)
    {
        _owner = owner;
        _cardRect = card;
        _imageRect = image;
        _shadowRect = shadow;
        _tilt = tilt;
        _hover = hover;
        _click = click;
        _move = move;

        _originalShadowPos = shadow.localPosition;
    }

    #region Play Idle (Tilt)
    private Coroutine _cotilt;
    private float _tiltStartOffset;
    public void PlayIdle(bool isRayCast = true)
    {
        _tiltStartOffset = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

        PlayTilt(_tilt.maxAngle, _tilt.maxAngle);

        MoveTo(_owner.SystemRectTransform, _owner.Card.OriginalPosition, isRayCast: false);
    }

    private void PlayTilt(float destAngle_X, float destAngle_Y)
    {
        StopTilt();
        _cotilt = _owner.StartCoroutine(CoTilt(destAngle_X, destAngle_Y));
    }

    IEnumerator CoTilt(float destAngle_X, float destAngle_Y)
    {
        while (true)
        {
            float time = Time.time + _tiltStartOffset;
            float sin = Mathf.Sin(time);
            float cos = Mathf.Cos(time);

            float newX = 0;
            float newY = 0;
            Vector3 euler = _imageRect.eulerAngles;

            switch (_owner.Card.CardState)
            {
                case ECardState.Idle:
                case ECardState.PointDown:
                case ECardState.Select:
                case ECardState.Moving:
                    {
                        newX = Mathf.LerpAngle(euler.x, sin * destAngle_X, _tilt.lerpSpeed * Time.deltaTime);
                        newY = Mathf.LerpAngle(euler.y, cos * destAngle_Y, _tilt.lerpSpeed * Time.deltaTime);
                    }
                    break;
                case ECardState.Hover:
                    {
                        Vector2 mousePos = Mouse.current.position.ReadValue();
                        Vector2 worldPos = _imageRect.position;

                        Vector2 dir = (worldPos - mousePos).normalized;

                        float tiltX = Mathf.Clamp(dir.y * _tilt.maxAngle, -_tilt.maxAngle, _tilt.maxAngle);
                        float tiltY = Mathf.Clamp(dir.x * _tilt.maxAngle, -_tilt.maxAngle, _tilt.maxAngle);

                        newX = Mathf.LerpAngle(euler.x, tiltX, _tilt.lerpSpeed * Time.deltaTime);
                        newY = Mathf.LerpAngle(euler.y, tiltY, _tilt.lerpSpeed * Time.deltaTime);
                    }
                    break;
            }

            _imageRect.eulerAngles = new Vector3(newX, newY, _imageRect.eulerAngles.z);
            _shadowRect.eulerAngles = new Vector3(newX, newY, _imageRect.eulerAngles.z);
            yield return null;
        }
    }
    #endregion

    #region Play Hover
    public void PlayHover()
    {
        if (_hover.IsApplyScaleAnimation)
            _cardRect.DOScale(_hover.scaleOnHover, _hover.scaleDuration).SetEase(Ease.OutBack);

        _imageRect
        .DOPunchRotation(Vector3.forward * _hover.hoverPunchAngle, _hover.hoverTransition, 20, 2)
        .OnComplete(() =>
        {
            PlayTilt(_tilt.maxAngle, _tilt.maxAngle);
        });
    }
    #endregion

    #region Play PointerDown
    public void PlayPointerDown()
    {
        if (_click.IsApplyScaleAnimation)
            _cardRect.DOScale(_click.scaleOnSelect, _click.scaleTransition).SetEase(Ease.OutBack).
                OnComplete(() =>
                {
                    if (_owner.Card.CardState == ECardState.PointDown)
                        PlayTilt(_tilt.hsMaxAngle, _tilt.hsMaxAngle);
                });

        _shadowRect.localPosition = _originalShadowPos + (-Vector3.up * _click.shadowOffset);
    }
    #endregion

    #region Play Select
    public void PlaySelect()
    {
        _cardRect.localPosition += (_cardRect.up * _click.selectPositionY_Offset);

        Sequence selectSequence = DOTween.Sequence();

        MoveTo(_owner.SystemRectTransform, _owner.Card.OriginalPosition, isRayCast: false);

        selectSequence
            .OnUpdate(() =>
            {
                // 이동 중에 Mouse로 격추했을 때 제자리로 돌아간 뒤에 Play Select Animation
                if (Util.IsMagnitudeEqual(_owner.SystemRectTransform.position, _owner.TargetPos, EPS: 0.9f) == false)
                    _owner.SystemRectTransform.position = Vector3.Lerp(_owner.SystemRectTransform.position, _owner.TargetPos, _move.followLerpSpeed * Time.deltaTime);
            })
            .Append(_imageRect.DOPunchPosition(_cardRect.up * _click.selectPunchAmount, _click.scaleTransition))
            .Join(_imageRect.DOPunchRotation(Vector3.forward * (_hover.hoverPunchAngle / 2), _hover.hoverTransition, 20, 1))
            .OnComplete(() =>
            {
                // Card 작아지고
                _cardRect.DOScale(Vector3.one, _hover.scaleDuration).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        PlayTilt(_tilt.hsMaxAngle, _tilt.hsMaxAngle);
                    });
            });

        selectSequence.Play();
    }
    #endregion

    #region Play Moving
    public void PlayMoving()
    {
        PlayTilt(_tilt.hsMaxAngle, _tilt.hsMaxAngle);

        PlayMovingUpdate();
    }

    private Coroutine _coRotationUpdate;
    public Coroutine _coPositionUpdate;
    public void PlayMovingUpdate()
    {
        _shadowRect.localPosition = _originalShadowPos + (-Vector3.up * _click.shadowOffset);

        StopMovingUpdate();
        _coRotationUpdate = _owner.StartCoroutine(CoCardRotationUpdate());
        _coPositionUpdate = _owner.StartCoroutine(CoCardFollowPosition());
    }

    IEnumerator CoCardRotationUpdate()
    {
        Vector3 lastInputPos = _owner.InputPosition;
        float targetZRotation = 0f;

        while (true)
        {
            Vector3 currentInputPos = _owner.InputPosition;
            float deltaX = lastInputPos.x - currentInputPos.x;
            lastInputPos = currentInputPos;

            float speed = Mathf.Abs(deltaX) / Time.deltaTime;
            bool isMoving = speed > _move.minMoveThreshold;

            float currentZ = NormalizeAngle(_cardRect.eulerAngles.z);

            if (isMoving)
            {
                float dynamicLerpSpeed = Mathf.Clamp(speed * 0.01f, _move.minLerpSpeed, _move.maxLerpSpeed);
                targetZRotation = Mathf.Clamp(deltaX * _move.baseSensitivity, -_move.maxZRotation, _move.maxZRotation);
                float smoothedZ = Mathf.Lerp(currentZ, targetZRotation, dynamicLerpSpeed * Time.deltaTime);
                _cardRect.eulerAngles = new Vector3(_cardRect.eulerAngles.x, _cardRect.eulerAngles.y, smoothedZ);
            }
            else
            {
                float smoothedZ = Mathf.Lerp(currentZ, 0f, _move.restoreLerpSpeed * Time.deltaTime);
                _cardRect.eulerAngles = new Vector3(_cardRect.eulerAngles.x, _cardRect.eulerAngles.y, smoothedZ);
            }

            yield return null;
        }
    }

    public void MoveTo(RectTransform rect, Vector3 destPos, bool isRayCast = true)
    {
        if (Util.IsMagnitudeEqual(rect.position, destPos, EPS: 0.9f))
            return;

        // Start Move -> 목적지에 도달하면 Stop Move
        _owner.Card.CardState = ECardState.Moving;
        StopMovingUpdate();
        _coPositionUpdate = _owner.StartCoroutine(CoCardFollowPosition(rect, destPos, () =>
        {
            if (Util.IsMagnitudeEqual(rect.position, destPos, EPS: 0.9f))
            {
                _owner.Card.CardState = ECardState.Idle;
                rect.position = destPos;
                StopMovingUpdate();
            }
        }));
    }

    // Move Force
    private IEnumerator CoCardFollowPosition(RectTransform rect, Vector3 DestPos, Action endFunc = null)
    {
        while (true)
        {
            rect.position = Vector3.Lerp(rect.position, DestPos, _move.followLerpSpeed * Time.deltaTime);

            yield return null;

            endFunc?.Invoke();
        }
    }
    // TargetPos까지 이동
    private IEnumerator CoCardFollowPosition(Action endFunc = null)
    {
        while (true)
        {
            _owner.SystemRectTransform.position = Vector3.Lerp(
                _owner.SystemRectTransform.position,
                _owner.TargetPos,
                _move.followLerpSpeed * Time.deltaTime);

            yield return null;

            endFunc?.Invoke();
        }
    }

    private float NormalizeAngle(float angle)
    {
        return (angle > 180f) ? angle - 360f : angle;
    }
    #endregion

    #region Reset Defalt
    public void ResetToIdle()
    {
        StopMovingUpdate();
        DOTween.Kill(_imageRect);

        // Card
        if (Util.IsMagnitudeEqual(_cardRect.localScale, Vector3.one) == false && _owner.Card.CardState != ECardState.Moving)
            _cardRect.DOScale(Vector3.one, _hover.scaleDuration).SetEase(Ease.OutBack);
        if (_owner.Card.CardState != ECardState.PointDown)
            _cardRect.localPosition = Vector3.zero;
        _cardRect.rotation = Quaternion.identity;

        // Image
        _imageRect.localScale = Vector3.one;
        _imageRect.localPosition = Vector3.zero;
        _imageRect.rotation = Quaternion.identity;

        // Shadow
        _shadowRect.localPosition = _originalShadowPos;
        _shadowRect.rotation = Quaternion.identity;
    }

    public void StopAllAnimation()
    {
        StopTilt();
        StopMovingUpdate();
    }

    public void StopTilt()
    {
        if (_cotilt == null)
            return;

        _owner.StopCoroutine(_cotilt);
        _cotilt = null;
    }

    public void StopMovingUpdate()
    {
        StopRotate();
        StopMove();
    }

    public void StopRotate()
    {
        if (_coRotationUpdate == null)
            return;

        _owner.StopCoroutine(_coRotationUpdate);
        _coRotationUpdate = null;
    }

    public void StopMove()
    {
        if (_coPositionUpdate == null)
            return;

        _owner.StopCoroutine(_coPositionUpdate);
        _coPositionUpdate = null;
    }
    #endregion
}
