// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;
using System;

namespace Fungus
{
    /// <summary>
    /// Transitions a sprite from its current color to a target color.
    /// An offset can be applied to slide the sprite in while changing color.
    /// </summary>
    [RequireComponent (typeof (Image))]
    public class SpriteFader : MonoBehaviour 
    {
        protected float fadeDuration;
        protected float fadeTimer;
        protected Color startColor;
        protected Color endColor;
        protected Vector2 slideOffset;
        protected Vector3 endPosition;

        protected Image image;

        protected Action onFadeComplete;

        protected virtual void Start()
        {
            image = GetComponent<Image>();
        }

        protected virtual void Update() 
        {
            fadeTimer += Time.deltaTime;
            
            if (fadeTimer > fadeDuration)
            {
                // Snap to final values
                image.color = endColor;
                if (slideOffset.magnitude > 0)
                {
                    transform.position = endPosition;
                }

                // Remove this component when transition is complete
                Destroy(this);

                if (onFadeComplete != null)
                {
                    onFadeComplete();
                }
            }
            else
            {
                float t = Mathf.SmoothStep(0, 1, fadeTimer / fadeDuration);
                image.color = Color.Lerp(startColor, endColor, t);
                if (slideOffset.magnitude > 0)
                {
                    Vector3 startPosition = endPosition;
                    startPosition.x += slideOffset.x;
                    startPosition.y += slideOffset.y;
                    transform.position = Vector3.Lerp(startPosition, endPosition, t);
                }
            }
        }

        #region Public members

        /// <summary>
        /// Attaches a SpriteFader component to a sprite object to transition its color over time.
        /// </summary>
        public static void FadeSprite(Image _image, Color targetColor, float duration, Vector2 slideOffset, Action onComplete = null)
        {
            if (_image == null)
            {
                Debug.LogError("Sprite renderer must not be null");
                return;
            }

            // Fade child sprite renderers
            var images = _image.gameObject.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                var sr = images[i];
                if (sr == _image)
                {
                    continue;
                }
                FadeSprite(sr, targetColor, duration, slideOffset);
            }

            // Destroy any existing fader component
            SpriteFader oldSpriteFader = _image.GetComponent<SpriteFader>();
            if (oldSpriteFader != null)
            {
                Destroy(oldSpriteFader);
            }

            // Early out if duration is zero
            if (Mathf.Approximately(duration, 0f))
            {
                _image.color = targetColor;
                if (onComplete != null)
                {
                    onComplete();
                }
                return;
            }

            // Set up color transition to be applied during update
            SpriteFader spriteFader = _image.gameObject.AddComponent<SpriteFader>();
            spriteFader.fadeDuration = duration;
            spriteFader.startColor = _image.color;
            spriteFader.endColor = targetColor;
            spriteFader.endPosition = _image.transform.position;
            spriteFader.slideOffset = slideOffset;
            spriteFader.onFadeComplete = onComplete;
        }

        #endregion
    }
}
