using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{

    #region Variables
    public static AnimationManager m_singleton;


    [SerializeField]
    private Material m_flashMaterial;

    private Shader m_shaderNormal;
    private Shader m_shaderDamaged;
    #endregion

    #region Initialization
    private void Awake() {
        if (m_singleton) {
            Destroy(gameObject);
            return;
        }
        m_singleton = this;

        m_shaderNormal = Shader.Find("Sprites/Default");
        m_shaderDamaged = Shader.Find("GUI/Text Shader");
    }
    #endregion

    #region Callers
    public void Hurt(Image sprite) {
        StartCoroutine(HurtAnimation(sprite));
    }

    public void Hurt(SpriteRenderer sprite) {
        StartCoroutine(HurtAnimation(sprite));
    }

    public void Death(Image sprite) {
        StartCoroutine(DeathAnimation(sprite));
    }

    public void Death(SpriteRenderer sprite) {
        StartCoroutine(DeathAnimation(sprite));
    }
    #endregion

    #region Coroutines
    IEnumerator HurtAnimation(Image sprite) {
        // Go white
        DamagedSprite(sprite);

        // Shaking
        Vector3 defaultPosition = transform.position;
        System.Random r = new System.Random();
        for (int i = 0; i < 5; i++) {
            double horizontalOffset = r.NextDouble() * 0.2 - 0.1f;
            Vector3 vectorOffset = new Vector3((float)horizontalOffset, 0, 0);
            transform.position += vectorOffset;
            yield return new WaitForSeconds(0.025f);
            transform.position = defaultPosition;
        }

        // Go normal
        NormalSprite(sprite);
    }

    IEnumerator HurtAnimation(SpriteRenderer sprite) {
        // Go white
        DamagedSprite(sprite);

        // Shaking
        Vector3 defaultPosition = transform.position;
        System.Random r = new System.Random();
        for (int i = 0; i < 5; i++) {
            double horizontalOffset = r.NextDouble() * 0.2 - 0.1f;
            Vector3 vectorOffset = new Vector3((float)horizontalOffset, 0, 0);
            transform.position += vectorOffset;
            yield return new WaitForSeconds(0.025f);
            transform.position = defaultPosition;
        }

        // Go normal
        NormalSprite(sprite);
    }

    IEnumerator DeathAnimation(Image sprite) {
        // loop over 0.5 second backwards
        for (float i = 0.25f; i >= 0; i -= Time.deltaTime) {
            // set color with i as alpha
            sprite.color = new Color(1, 1, 1, i);
            transform.localScale = new Vector3(1.5f - i, 1.5f - i, 1);
            yield return null;
        }

        sprite.color = new Color(1, 1, 1, 1);
        transform.localScale = new Vector3(1, 1, 1);
        sprite.gameObject.SetActive(false);
    }

    IEnumerator DeathAnimation(SpriteRenderer sprite) {
        // loop over 0.5 second backwards
        for (float i = 0.25f; i >= 0; i -= Time.deltaTime) {
            // set color with i as alpha
            sprite.color = new Color(1, 1, 1, i);
            transform.localScale = new Vector3(1.5f - i, 1.5f - i, 1);
            yield return null;
        }

        sprite.color = new Color(1, 1, 1, 1);
        transform.localScale = new Vector3(1, 1, 1);
        sprite.gameObject.SetActive(false);
    }
    #endregion

    #region Helper
    void DamagedSprite(Image sprite) {
        sprite.material = m_flashMaterial;
        sprite.color = Color.white;
    }

    void DamagedSprite(SpriteRenderer sprite) {
        sprite.material.shader = m_shaderDamaged;
        sprite.color = Color.white;
    }

    void NormalSprite(Image sprite) {
        sprite.material = null;
        sprite.color = Color.white;
    }

    void NormalSprite(SpriteRenderer sprite) {
        sprite.material.shader = m_shaderNormal;
        sprite.color = Color.white;
    }
    #endregion
}
