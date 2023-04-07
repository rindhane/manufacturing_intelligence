/*
<li class="app-link-outer"><a class="app-link" href="/MainDashboard?Line=LineABC">Line: ABC</a></li>
<li class="app-link-outer"><a class="app-link" href="/DemoDashboard">Line:DEMO</a></li>
*/
const DashContainer = document.getElementById("app-links");
const uploadFinishModal= document.getElementById('FinishModal');
const FinishModalText= document.getElementById("FinishModalText");

async function populateDashboardApps(MasterElem){
    let dashs= await postDataStream("/GetDashboardLines");
    dashs.forEach(element => {
      MasterElem.appendChild( createDashElem(element));
    });
    MasterElem.appendChild(createDemoDashElem());
    return true;
}

async function postDataStream(url){ 
    let options={
      method: 'POST',
      headers: {
        Accept: 'application.json',
        'Content-Type': 'text/plain',
      },
      body: 'GetDashboardLines',
      cache: 'default',
    }
    let response = await fetch(url, options); //`${serverMainPath}/partData`
    const data = await response.json();
    return data;
  };

function createDashElem(LineName){
  const outerElem=document.createElement('li');
  const innerElem=document.createElement('a');
  outerElem.setAttribute('class',"app-link-outer");
  innerElem.setAttribute('class',"app-link");
  innerElem.href=`/MainDashboard?Line=${LineName}`;
  innerElem.innerHTML=`Line: ${LineName}`;
  outerElem.appendChild(innerElem);
  return outerElem;
}

function createDemoDashElem(LineName="DEMO"){
  const outerElem=document.createElement('li');
  const innerElem=document.createElement('a');
  outerElem.setAttribute('class',"app-link-outer");
  innerElem.setAttribute('class',"app-link");
  innerElem.href=`/DemoDashboard?Line=${LineName}`;
  innerElem.innerHTML=`Line: ${LineName}`;
  outerElem.appendChild(innerElem);
  return outerElem;
}

//populate the dashboard type apps
populateDashboardApps(DashContainer);

async function syncControlPlan(){
  //console.log("synced up!");
  let response = await sendSyncUpSignal();
  //notifytheUpdate(response);
  notifytheUpdate(response);
  uploadFinishModal.style.display='block'; //global variable usage
}

//confirmation of sync
function notifytheUpdate(text,elem=FinishModalText){
  if(text!=="True"){
    elem.innerText="Error : Something went wrong";
    return false;
  }
  
  elem.innerText="Done: Sync-up is completed";
  return true;
}
//function to get the lab stations configured in backend
async function sendSyncUpSignal(){
  let options={
    method: 'POST',
    headers: {
      Accept: 'application.json',
      'Content-Type': 'text/plain',
    },
    body: "Sync Up the part Codes",
    cache: 'default',
  }
  const serverMainPath = '';//;"http://localhost:8080" ;
  let response = await fetch(`${serverMainPath}/SyncControlPlans`, options); 
  const data = await response.text();
  return data;
}

//
function reloadWindow(){
  location.reload();
};