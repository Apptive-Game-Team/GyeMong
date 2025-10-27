using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.UISystem.Social;
using UnityEngine;

public class SocialPage : MonoBehaviour
{
    [SerializeField] private GameObject loginPage;
    [SerializeField] private GameObject autoLoginPage;
    [SerializeField] private GameObject joinPage;
    [SerializeField] private GameObject myPage;
    [SerializeField] private GameObject loadingPage;
    
    Dictionary<AccountContext.PageType, GameObject> pages = new();
    
    private AccountContext.PageType currentPage = AccountContext.PageType.AutoLogin;
    
    private void Awake()
    {
        pages.Add(AccountContext.PageType.LOGIN, loginPage);
        pages.Add(AccountContext.PageType.JOIN, joinPage);
        pages.Add(AccountContext.PageType.MY, myPage);
        pages.Add(AccountContext.PageType.LOADING, loadingPage);
        pages.Add(AccountContext.PageType.AutoLogin, autoLoginPage);
    }

    private void Update()
    {
        // Debug.Log($"{AccountContext.IsLoading} / {AccountContext.CurrentPage} / {currentPage}");
        if (pages.Count == 0) return;
        
        if (AccountContext.IsLoading)
        {
            if (currentPage != AccountContext.PageType.LOADING)
            {
                ShowPage(AccountContext.PageType.LOADING);
            }
        }
        else
        {
            if (currentPage != AccountContext.CurrentPage)
            {
                ShowPage(AccountContext.CurrentPage);
            }
        }
    }

    private void ShowPage(AccountContext.PageType type)
    {
        foreach (var page in pages)
        {
            page.Value.SetActive(false);
        }
        
        pages[type].SetActive(true);
        currentPage = type;
    }
}
