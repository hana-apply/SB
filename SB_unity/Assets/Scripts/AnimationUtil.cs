using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationUtil : MonoBehaviour
{
    [SerializeField, Range(0.0f, 1.0f)]
    float _delayRatio = 0f;

    [SerializeField, Range(0.0f, 2.0f)]
    float _minSpeed = 1f;
    [SerializeField, Range(0.0f, 2.0f)]
    float _maxSpeed = 1f;
    
    Animator _animator = null;

    void Awake()
    {
        _animator = GetComponent<Animator>();   
    }

    void Start()
    {
        delayStartAnimation();
    }
    private void Update()
    {
        var info = _animator.GetCurrentAnimatorStateInfo(0);
        if( info.normalizedTime > 1f )
        {
            _animator.speed = Random.Range(_minSpeed, _maxSpeed);
            _animator.Play(info.fullPathHash, 0, info.normalizedTime - 1f );
        }
    }

    void OnEnable()
    {
        delayStartAnimation();
    }

    void delayStartAnimation()
    {
        var info = _animator.GetCurrentAnimatorStateInfo(0);
        _animator.Play(info.fullPathHash, 0, _delayRatio);
    }
}
