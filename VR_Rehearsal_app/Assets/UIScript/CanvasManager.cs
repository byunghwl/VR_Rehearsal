﻿using UnityEngine;
using System.Collections;

public class CanvasManager : MonoBehaviour {

    public GameObject logo;
    public GameObject localShowCase;
    public GameObject fileTranser;
    public GameObject navigation;
    public GameObject loading;
    public GameObject customize;
    public GameObject calibration;
    public GameObject rotation;
	public GameObject showCaseButton;

    static public bool againTrigger = false;
    static public bool finishTrigger = false;

	private SetupManager _setupManager;
    bShowcaseManager.showcase_Data customData;
    private string _showCaseName;
    private int _sizeOfRoom;
    private int _numberOfAudience;
    private string _localPath;
    private string _id;
    private int _expectedTime;
    public bool isFromCustom;

    public void SetData(string showCanseName, int sizeOfRoom, int numberOfAudience, string localPath, string id, int time)
    {
        _showCaseName = showCanseName;
        _sizeOfRoom = sizeOfRoom;
        _numberOfAudience = numberOfAudience;
        _localPath = localPath;
        _id = id;
        _expectedTime = time;
    }
    public string GetShowCaseName()
    {
        return _showCaseName;
    }
    public int GetRoom()
    {
        return _sizeOfRoom;
    }
    public int GetAudience()
    {
        return _numberOfAudience;
    }
    public string GetLocalPath()
    {
        return _localPath;
    }
    public string GetPPTID()
    {
        return _id;
    }
    public int GetTime()
    {
        return _expectedTime;
    }
    public void SetPPTID(string str)
    {
        _id = str;
        GetDataByID(_id);
    }
    public void GetDataByID(string pptID)
    {
        bShowcaseManager.showcase_Data? tempShocase = _setupManager.BShowcaseMgr.GetSignleShowcase(_id);
        if (tempShocase != null) {

            SetData(tempShocase.Value._showcaseName, (int)tempShocase.Value._mapIdx, (int)tempShocase.Value._percentageOfAudience, tempShocase.Value._pptFolderPath, tempShocase.Value._showcaseID, (int)tempShocase.Value._expetedTime_min);      
        }
        
    }
    void Awake () {
        localShowCase.SetActive(false);
        fileTranser.SetActive(false);
        navigation.SetActive(false);
        loading.SetActive(false);
        customize.SetActive(false);
        _setupManager = new SetupManager();
        localShowCase.GetComponent<LocalCaseView>().SetSetupManager(_setupManager);
        navigation.GetComponent<NavigationView>().SetSetupManager(_setupManager);
        customize.GetComponent<CustomizeView>().SetSetupManager(_setupManager);
        rotation.GetComponent<RotationView>().SetSetupManager(_setupManager);
        calibration.SetActive(false);
        rotation.SetActive(false);
    }
   public void SetisFromCustom(bool res)
    {
        isFromCustom = res;
    }
    public bool GetisFromCustom()
    {
        return isFromCustom;
        
    }
   public void PlayAgainButtonClicked()
    {
        if(againTrigger == true)
        {
            ShowRotationView();
        }

    }
    void Update () {

        ShowLocasShowView();
        ShowFileTransferView();
        ShowNavigationView();
        ShowCustomView();
        ShowCalibrationView();
        ShowRotationView();
        NavigationBetweenView();
        ShowCustomViewFromLocalCaseView();
        ShowCustomViewToLocalView();

    }
    public void ShowLocasShowView()
    {
        if (LogoView.isLogoSceneDone)
        {
            localShowCase.SetActive(true);
            LogoView.isLogoSceneDone = false;
            fileTranser.SetActive(false);
        }
        else if(finishTrigger == true)
        {
            localShowCase.SetActive(true);
            LogoView.isLogoSceneDone = false;
            fileTranser.SetActive(false);
            finishTrigger = false;
        }
        else if (againTrigger ==true)
        {
            localShowCase.SetActive(false);
        }
    }

    public void ShowFileTransferView()
    {
        if (LocalCaseView.isLocalCaseDone && localShowCase.GetComponent<LocalCaseView>().isFileTransferClicked==true)
        {
            fileTranser.SetActive(true);
            LocalCaseView.isLocalCaseDone = false;
            localShowCase.GetComponent<LocalCaseView>().isFileTransferClicked = false;
        }
    }
    public void ShowNavigationView()
    {
        if (FileTransferView.isFileTransferViewDone)
        {
            int transferType = fileTranser.GetComponent<FileTransferView>().transferNumber;
            navigation.SetActive(true);
            Debug.Log(transferType);
            navigation.GetComponent<NavigationView>().SetupCloud(transferType);
            FileTransferView.isFileTransferViewDone = false;
        }
    }
    public void ShowCustomView()
    {
         if (NavigationView.isNavigationDone)
        {
            NavigationView.isNavigationDone = false;
            customize.SetActive(true);
            loading.SetActive(false);
        }
    }
   public void ShowCustomViewFromLocalCaseView()
    {
        if (LocalCaseView.isCustomizeButtonClicked)
        {
            LocalCaseView.isCustomizeButtonClicked = false;
            localShowCase.SetActive(false);
            customize.SetActive(true);
        }
    }
    public void ShowCustomViewToLocalView()
    {

        if (Input.GetKey(KeyCode.Escape) && customize.GetComponent<CustomizeView>().isCustomizeDoneFromLocal == true)
        {
                customize.SetActive(false);
                localShowCase.SetActive(true);
                customize.GetComponent<CustomizeView>().isCustomizeDoneFromLocal = false;
          
        }
    }
    public void ShowCalibrationView()
    {
        if (CustomizeView.isCustomizeDone)
        {
            CustomizeView.isCustomizeDone = false;
            calibration.SetActive(true);
        }
       
    }
    public void ShowRotationView()
    {
		
        //if (CalibrationView.isCalibrationDone)
        if (CalibrationControlNew.isCalibrationDone)
        {
            //CalibrationView.isCalibrationDone = false;
            CalibrationControlNew.isCalibrationDone = false;
            rotation.SetActive(true);
            rotation.GetComponent<RotationView>().SetRotation(true);
        }
        else if(againTrigger == true)
        {

            rotation.SetActive(true);
            localShowCase.SetActive(false);
            rotation.GetComponent<RotationView>().SetRotation(true);

        }
    }
    public void DirectShowCalibrationView()
    {
        foreach (GameObject temp in localShowCase.GetComponent<LocalCaseView>().storedShowCase)
        {
            if (temp.GetComponent<ShowCaseButton>().isShowcaseButtonClicked == true)
            {
                localShowCase.SetActive(false);
                calibration.SetActive(true);
            }

        }
    }
    /*
	public void DirectShowRotationView(){
        foreach(GameObject temp in localShowCase.GetComponent<LocalCaseView>().storedShowCase)
        {
            if(temp.GetComponent<ShowCaseButton>().isShowcaseButtonClicked == true)
            {
                localShowCase.SetActive(false);
                rotation.SetActive(true);
                rotation.GetComponent<RotationView>().SetRotation(true);
            }

        }
	}
    */
      void NavigationBetweenView()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (fileTranser.activeSelf)
            {
                fileTranser.SetActive(false);
                FileTransferView.isFileTransferViewDone = false;
                localShowCase.SetActive(true);
            }
            if (navigation.activeSelf)
            {
				//1. Auth Failed Case
				if (!navigation.GetComponent<NavigationView> ().GetAuthenticationStatus ()) {
					navigation.SetActive(false);
					NavigationView.isNavigationDone = false;
					navigation.GetComponent<NavigationView>().DeletePanels(true, "ok");
					fileTranser.SetActive(true);
					return;
				}
					
				//2. Back button Keep pushing case
                string str = navigation.GetComponent<NavigationView>().RecentPath();
                Debug.Log(str);
				if (str == "null") {
					return;
				}

                if (str == "/" || str == ""){
                    navigation.SetActive(false);
                    NavigationView.isNavigationDone = false;
                    navigation.GetComponent<NavigationView>().DeletePanels(true, "ok");
                    fileTranser.SetActive(true);
                }

            }
        }
    }


}
