﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenu : MonoBehaviour
{
    public RectTransform pointerPivot;

    public UISlot[] UISlots;
    private UISlot[] codedSlots;
    private UISlot currentlySelectedSlot;
    private int count;
    public GameObject prefab;
    private SpellCaster m_spellCaster;
    private HashSet<Spell> _addedSpells;
    List<KeyCode> keyBindings;
    
    // Start is called before the first frame update
    void Start()
    {
        keyBindings = new List<KeyCode>();
        keyBindings.Add(KeyCode.Mouse0);
        keyBindings.Add(KeyCode.Mouse1);
        keyBindings.Add(KeyCode.Q);
        keyBindings.Add(KeyCode.E);
        codedSlots = new UISlot[4];
        _addedSpells = new HashSet<Spell>();
        count = 0;
        InvokeRepeating( nameof(IntervalUpdate), 0f, 0.1f);

        m_spellCaster = GameManager.Instance._player.GetComponent<SpellCaster>();
    }

    public void UpdateMenu()
    {
        if (count > 9)
        {
            return;
        }
        foreach (var spell in Inventory.Instance._spells)
        {
            if (_addedSpells.Contains(spell))
            {
                Debug.Log(spell.name);
                return;
            }
            var newObj = Instantiate(prefab, transform);
            var uiItem = newObj.GetComponent<UIItem>();
            uiItem._spellItem = spell;
            uiItem.SetImage();
            UISlots[count++].Slot(uiItem);
            _addedSpells.Add(spell);
        }

        foreach (var slot in UISlots)
        {
            if (!slot.IsSlotted()) return;
            var slottedSpellItem = slot.GetSlottedSpellItem();
            int index = 0;
            foreach(var spell in m_spellCaster.spells)
            {
                if (slottedSpellItem == spell)
                {
                    slot.GetSlottedUiItem().SetKeyCode(keyBindings[index]);
                    codedSlots[index] = slot;
                }
                index++;
            }
        }
    }

    public void Update()
    {
        if (!CraftMenuManager.Instance.IsQuickCraftMenuDisplayed()) return;
        var index = -1;
        
        
        if (Input.GetKey(KeyCode.Q))
        {
            index = 2;
        }
        if (Input.GetKey(KeyCode.E))
        {
            index = 3;

        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            index = 0;
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            index = 1;
        }

        if (index < 0) return;

        var slottedUiItem = currentlySelectedSlot.GetSlottedUiItem();
        if (slottedUiItem == null) return;

        var key = keyBindings[index];
        var uiItem = codedSlots[index].GetSlottedUiItem();
        if (uiItem == slottedUiItem) return;
        uiItem.SetKeyCode(KeyCode.Clear);

        m_spellCaster.ClearSpell((Spell) uiItem._spellItem);
        slottedUiItem.SetKeyCode(key);
        m_spellCaster.SetSpell(index, (Spell) currentlySelectedSlot.GetSlottedSpellItem());
        int i = 0;
        foreach (var slot in codedSlots)
        {
            Debug.Log(slot.name + i++);
        }
        codedSlots[index] = currentlySelectedSlot;
    }

    // Update is called once per 0.1s
    void IntervalUpdate()
    {
        currentlySelectedSlot = GetClosest(UISlots, Input.mousePosition);
        var pointerDirection = currentlySelectedSlot.transform.position - pointerPivot.position;
        float targetRotation = 180 / Mathf.PI * Mathf.Atan2(pointerDirection.y,pointerDirection.x) + 90f;
        
        pointerPivot.eulerAngles = new Vector3(0,0, targetRotation);
        
    }

    private UISlot GetClosest(UISlot[] allSlots, Vector3 pos)
    {
        UISlot closest = null;
        float closeVal = Single.MaxValue;
        
        foreach (var slot in allSlots)
        {
            var position = slot.transform.position;
            var sqrMagnitude = (pos - position).sqrMagnitude;
            if (sqrMagnitude < closeVal)
            {
                closeVal = sqrMagnitude;
                closest = slot;
            }
        }

        return closest;
    }
}
    