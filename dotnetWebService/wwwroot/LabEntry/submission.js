const fileBearer = document.getElementById("fileInput");
const dropArea = document.getElementById('dropField');
const input = document.getElementById('SerialField');
const validationElem = document.getElementById('SerialFieldValidation');
const fileSelectDisplay = document.getElementById('resultFileDisplay');
const selectElem=document.getElementById('LabStation');
var fileHook = null;
const uploadFinishModal= document.getElementById('FinishModal');
const FinishModalText= document.getElementById("FinishModalText");


//If choose file option is selected
fileBearer.addEventListener('change', (event) => {
    const fileList = event.target.files;
    fileHook=LoadFileData(fileList);
    styleElemOnFileLoad(fileList);
  });

function styleElem (styleEnum,fileName,elem=dropArea, displayElem=fileSelectDisplay){

    if(styleEnum==1){ //styling of original elem
        elem.style.backgroundColor="#c8dadf";
        return 1; 
    }
    if(styleEnum==2){ //styling for dragover
        elem.style.backgroundColor='lightblue';
        return 1; 
    }
    if(styleEnum==3){ //styling for dropped or selection of file
        elem.style.backgroundColor='lightgreen';
        displayElem.style.display='block';
        displayElem.innerText=fileName;
        return 1; 
    }
    return 0; 
}
//drag & drop Eventlisteners
dropArea.addEventListener('dragover', (event) => {
    let elem = dropArea;
    event.stopPropagation();
    event.preventDefault();
    // Style the drag-and-drop as a "copy file" operation.
        //styling
        styleElem(2);
        //event 
        event.dataTransfer.dropEffect = 'copy';
  });
dropArea.addEventListener('drop', (event) => {
    let elem=dropArea;
    event.stopPropagation();
    event.preventDefault();
    //event action 
    const fileList = event.dataTransfer.files;
    fileHook=LoadFileData(fileList);
    styleElemOnFileLoad(fileList);
  });

dropArea.addEventListener('dragleave', (event)=>{
    let elem = dropArea;
    event.stopPropagation();
    event.preventDefault();
    // return back to the original styling of dropArea 
    styleElem(1);
})
//
//fileupload functions
function styleElemOnFileLoad(fileList){
  if(fileList.length>0){
    fileHandler = fileList[0]
      //styling
      styleElem(3,fileHandler.name);
      return true;
  }
  return false;
}
function LoadFileData(fileList){
    if(fileList.length>0){
      fileHandler = fileList[0];
      let temp = fileAsUrlBase64(fileHandler);       
      return temp;
    }
    return null;
};

function fileAsUrlBase64(file){
    const reader = new FileReader();
    let promise = new Promise((resolve,reject)=> {
    reader.addEventListener('load', (event) => {
        //uploadFileData(event.target.result);
        //console.log(event.target.result);
        resolve(event);
    });
  });
    reader.readAsDataURL(file);
    return promise;
};

//function getCookie(key) {
//    var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
//    return keyValue ? keyValue[2] : null;
//}
async function uploadFileData(fileHook, inputElem=input, checkElem=validationElem,){
    const serverMainPath = '';    //'http://127.0.0.1:5001';
    //var productselectedValue = $('#dropField').val();   
    //var selVal = selectElem.value;
    //document.cookie = "CURRENT_DROP_DOWN_VAL=" + selVal + ";expires=Thu, 01-Jan-2099 00:00:01 GMT;path=/";
    //alert(selVal);
    if (checkElem.style.display=="none" 
          || 
        checkElem.style.display=="" 
        || 
        checkElem.style.color=="red" 
        || 
        checkElem.style.color==''
        )
    {
      alert("provide valid serial number");
      return false;
    }
    if (selectElem.value=="0"){
      alert("select station");
      return false;
    }
    if (fileHook==null) {
      alert('Pdf not attached');
      return false;
    }
    let event = await fileHook; 
    let dat= extractBase64Data(event.target.result);
    response= await postDataStream(`${serverMainPath}/LabData`, uploadData= dat, serialNum=inputElem.value, 
                        LabStation=selectElem.value);
    notifytheUpdate(response);
    uploadFinishModal.style.display = 'block';

    return true;
};

async function postDataStream(url, uploadData, serialNum ,LabStation){ 
  let options={
    method: 'POST',
    headers: {
      Accept: 'application.json',
      'Content-Type': 'text/plain',
      serialNum:serialNum,
      LabStation:LabStation,
    },
    body: uploadData,
    cache: 'default',
  }
  let response = await fetch(url, options); //`${serverMainPath}/partData`
  const data = await response.text();
  return data;
};

function extractBase64Data(impureBase64){
    //remove the prefix: `data:*/*;base64,` infroto of impure base64
    refString='data:application/pdf;base64,';
    return impureBase64.slice(refString.length); 
}
//
//confirmation of upload
function notifytheUpdate(text,elem=FinishModalText){
  if(text==null || text=='') {
    elem.innerText="Error : Data was not uploaded";
    return false;
  }
  elem.innerText=text;
  console.log(text);
  return true;
}
//
//function to get the lab stations configured in backend
async function getLabStations(){
  let options={
    method: 'POST',
    headers: {
      Accept: 'application.json',
      'Content-Type': 'text/plain',
    },
    body: "GetLabStations",
    cache: 'default',
  }
  const serverMainPath = '';//"http://localhost:8080" ; //
  let response = await fetch(`${serverMainPath}/GetLabStations`, options); //`${serverMainPath}/GetLabStations`
  const data = await response.text();
  return data;
}

async function populateSelector(selElem, data) {
   // debugger;
  let LabList = JSON.parse(data);
  selElem.appendChild(createElementForSelector("0","Select Station"));
  LabList.forEach(elem => {
    selElem.appendChild(createElementForSelector(elem,elem));
  });

   // var elementid = localStorage.getItem('selectElemid');
    //if (elementid !== 'undefined' && elementid !== null) {      
    document.getElementById("LabStation").selectedIndex = parseInt(localStorage.getItem('selectElemid'));
        //localStorage.clear();
    //}
}

function createElementForSelector(value,text){
  // ref element => <option value="0">Select Station</option>
  let elem = document.createElement("option");
  elem.setAttribute("value",value);
  elem.innerText=text;
  return elem;
}

//
//function reloadWindow(inputElem=input){
//  inputElem.value='';
//   selectElem.value = 0;
  // location.reload();
//};
//$(document).ready(function () {
//    debugger;
//    $('#LabStation').value(localStorage.getItem('selectElemid'));
//})
//(function () {
//    debugger;
//   // document.getElementsById('#LabStation').value(localStorage.getItem('selectElemid'));

//    var abc = localStorage.getItem('selectElemid');

//    if (abc !== 'undefined' && abc !== null) {
//        var selectedlab = localStorage.getItem('selectElemid');
//        document.getElementById("LabStation").selectedIndex = parseInt(selectedlab);
//        localStorage.clear();
//    }    
//    })();
function reloadWindow(inputElem = input) {
   debugger;
    inputElem.value = '';
   // selectElem.value = 0;
    localStorage.setItem("selectElemid", selectElem.selectedIndex);      
    location.reload();   
};
//selectElement = document.querySelector('#select1');
//output = selectElement.value;
//document.querySelector('.output').textContent = output;

//function getOption() {
//    selectElement = document.querySelector('#LabStation');
//    output = selectElement.value;
//    document.querySelector('.output').textContent = output;


//function setLocalStorageValue() {
//   // let selectElem = document.getElementById('LabStation');
//    let myNewValue = selectElem.value;

//    let localStorage = window.localStorage;
//    localStorage.setItem('defaultValue', myNewValue);
//}

//function getLocalStoredValue() {
//    let localStorage = window.localStorage;
//    let defaultValue = json.parse(localStorage.getItem('defaultValue'));   
//    selectElem.value = defaultValue;
//}
//input validation 
input.addEventListener('input', triggerValidation);
function triggerValidation(e) {
    const value = e.target.value;
    const status = validateSerialNumber(value);
    if(value!=""){
        showValidationMessage(validationElem,status);
        return 1;
    }
    showValidationMessage(validationElem,'hide');
    return 1;
}

function validateSerialNumber(string){
    const regexChecker = /^[0-9]{7};[0-9]{5};[1-3]{1};[a-zA-Z0-9]{5};[PG]{1}[0-9]{2};[a-zA-Z0-9]{5};[0-9]{2}:[0-9]{2}:[0-9]{2}$/i ;
    return regexChecker.test(string);
  }

function showValidationMessage(cntxt,status){
    cntxt.style.display='inline';
    if (status==true){
      cntxt.style.color='green';
      cntxt.textContent="Valid Serial Number";
      return 1;
    }
    if (status=='hide'){
        cntxt.style.display='none';
        return 1;
    }
    cntxt.style.color='red';
    cntxt.textContent="Invalid Serial Number";
    return 0;
}

/*
Understanding ref : 
    1.  [Stream in Post ] : https://developer.chrome.com/articles/fetch-streaming-requests/
        [Readable Stream ] : https://developer.mozilla.org/en-US/docs/Web/API/ReadableStream
    2.  [Iterators in Js]https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Iterators_and_Generators 

*/

/*
// effort for readable Stream;
function wait(milliseconds) {
    return new Promise(resolve => setTimeout(resolve, milliseconds));
  }
  
const stream = new ReadableStream({
    async start( controller) {
      await wait(1000);
      controller.enqueue('This ');
      await wait(1000);
      controller.enqueue('is ');
      await wait(1000);
      controller.enqueue('a ');
      await wait(1000);
      controller.enqueue('slow ');
      await wait(1000);
      controller.enqueue('request.');
      controller.close();
    },
}).pipeThrough(new TextEncoderStream());

async function postDataStream(url, stream){ 
    let options={
      method: 'POST',
      method: 'POST',
      headers: {
        Accept: 'application.json',
        'Content-Type': 'text/plain'
      },
      body: stream,
      //cache: 'default',
      duplex: 'half'
    }
    let response = await fetch(url, options); //`${serverMainPath}/partData`
    const data = await response.text();
    console.log(data);
    return data;
  }

const serverMainPath='http://127.0.0.1:5001' ;
( async () =>{
    await postDataStream(`${serverMainPath}/LabData`, stream);
}

)();

*/


