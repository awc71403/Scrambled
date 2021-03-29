using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Aka Word
public class Unit: MonoBehaviour
{
    #region Variables
    private Tile[] m_letters;
    private Tile m_firstLetter;
    private int m_unitLength;

    private int m_damage;
    private int m_maxHealth;
    private int m_currHealth;

    private SpriteRenderer m_spriteRenderer;
    private Shader m_shaderNormal;
    private Shader m_shaderDamaged;

    private bool m_token;

    [SerializeField]
    private TileDatabaseSO.TileData.Trait[] m_traits;

    private bool m_isHorizontal;
    #endregion

    #region Initialization
    private void Awake() {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_shaderNormal = Shader.Find("Sprites/Default");
        m_shaderDamaged = Shader.Find("GUI/Text Shader");
    }

    public void Setup(Tile[] tiles, bool isHorizontal) {
        Debug.Log("Unit Setup");
        m_damage = 0;
        m_maxHealth = 0;
        m_token = false;

        m_letters = tiles;
        m_firstLetter = tiles[0];

        m_traits = new TileDatabaseSO.TileData.Trait[3];
        m_traits[0] = m_firstLetter.GetData.m_trait1;
        m_traits[1] = m_firstLetter.GetData.m_trait2;
        m_traits[2] = m_firstLetter.GetData.m_trait3;

        SetLocation(m_firstLetter);

        m_unitLength = m_letters.Length;
        m_isHorizontal = isHorizontal;
        if (m_unitLength == 1) {
            Debug.Log($"{tiles[0].GetName} is a single tile");
            tiles[0].IsSingleTile = true;
        }
        else if (isHorizontal) {
            tiles[0].IsFirstHorizontal = true;
        }
        else {
            tiles[0].IsFirstVertical = true;
        }

        foreach (Tile tile in tiles) {
            if (isHorizontal) {
                tile.HorizontalDamage = m_unitLength;
                tile.HorizontalHealth = m_unitLength;
                tile.HorizontalUnit = this;
            }
            else {
                tile.VerticalDamage = m_unitLength;
                tile.VerticalHealth = m_unitLength;
                tile.VerticalUnit = this;
            }

            m_damage += tile.GetDamage + tile.VerticalDamage + tile.HorizontalDamage;
            m_maxHealth += tile.GetHealth + tile.VerticalHealth + tile.HorizontalHealth;
        }

        m_currHealth = m_maxHealth;
    }
    #endregion

    #region Update
    public void UpdateWord() {

    }
    #endregion

    #region Getter
    public Tile GetFirstLetter {
        get { return m_firstLetter; }
    }

    public TileDatabaseSO.TileData.Trait[] GetTraits {
        get { return m_traits; }
    }

    public Tile[] GetLetters {
        get { return m_letters; }
    }

    public int GetUnitLength {
        get { return m_unitLength; }
    }

    public int GetDamage {
        get { return m_damage; }
    }
    #endregion

    #region Transform
    private void SetLocation(Tile tile) {
        transform.SetParent(tile.transform);
        transform.position = tile.transform.position;
    }
    #endregion

    #region Battle
    //Returns if dead
    public bool TakeDamage(int damage) {
        m_currHealth -= damage;
        if (m_currHealth > 0) {
            StartCoroutine(HurtAnimation());
            return false;
        }
        else {
            StartCoroutine(DeathAnimation());
            return true;
        }
    }
    #endregion

    #region Coroutine
    void DamagedSprite() {
        m_spriteRenderer.material.shader = m_shaderDamaged;
        m_spriteRenderer.color = Color.white;
    }

    void NormalSprite() {
        m_spriteRenderer.material.shader = m_shaderNormal;
        m_spriteRenderer.color = Color.white;
    }

    IEnumerator HurtAnimation() {
        // Go white
        DamagedSprite();

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
        NormalSprite();
    }

    IEnumerator DeathAnimation() {
        // loop over 0.5 second backwards
        print("death time");
        for (float i = 0.25f; i >= 0; i -= Time.deltaTime) {
            // set color with i as alpha
            m_spriteRenderer.color = new Color(1, 1, 1, i);
            transform.localScale = new Vector3(1.5f - i, 1.5f - i, 1);
            yield return null;
        }

        m_spriteRenderer.color = new Color(1, 1, 1, 1);
        transform.localScale = new Vector3(1, 1, 1);
        gameObject.SetActive(false);
    }
    #endregion
}
