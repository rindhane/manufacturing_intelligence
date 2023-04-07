
async function makePostCall(){
  //console.log("synced up!");
  let response = await sendSyncUpSignal();
}

async function postFunction(){
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

