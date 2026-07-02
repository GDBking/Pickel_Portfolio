using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteOutline))]
[RequireComponent(typeof(Animator))]
public class VillageNPC : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    [SerializeField] int buildingIdx;
    [SerializeField] OrePieceData pieceData;

    [Header("Panel")]
    [SerializeField] Sprite sprite;
    [SerializeField] string buildingName;

    [Header("기본 텍스트(Fallback)")]
    [SerializeField] [TextArea(10, 100)] string buildingDecs;

    [Header("다국어 번역 리스트")]
    [SerializeField] private List<LocalizationData> localizedDescriptions;

    SpriteOutline outline;

    private void Awake()
    {
        outline = GetComponent<SpriteOutline>();
        GetComponent<Animator>().SetBool(IdleHash, true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (VillageManager.Instance.buildingList[buildingIdx].activeSelf) return;

        // 🌟 싱글톤 매니저에게 내 다국어 리스트를 넘겨서 현재 언어에 맞는 텍스트를 받아옵니다.
        string translatedName = Localization.instance.GetName(localizedDescriptions, buildingName);
        string translatedDesc = Localization.instance.GetDescription(localizedDescriptions, buildingDecs);

        // 패널 오픈
        BuildingPanel.Instance.Init(sprite, translatedName, translatedDesc, pieceData, buildingIdx);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (VillageManager.Instance.buildingList[buildingIdx].activeSelf) return;

        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (VillageManager.Instance.buildingList[buildingIdx].activeSelf) return;

        outline.enabled = false;
    }

    public string GetDescription(string langCode)
    {
        // 리스트가 비어있거나 null인 경우 기본 텍스트 반환
        if (localizedDescriptions == null || localizedDescriptions.Count == 0)
            return buildingDecs;

        // 리스트에서 대소문자 구분 없이 언어 코드 검색 ("En"과 "en", "EN" 모두 매칭됨)
        var result = localizedDescriptions.Find(x => x.languageCode.Equals(langCode, StringComparison.OrdinalIgnoreCase));

        // 찾지 못했거나 내용이 비어있다면 기본 텍스트 반환, 있다면 번역본 반환
        return string.IsNullOrEmpty(result.description) ? buildingDecs : result.description;
    }
}