const input = document.getElementById('SerialField');
const validationElem = document.getElementById('SerialFieldValidation');
const LineSelector = document.getElementById('LineSelector');
const OperationSelector = document.getElementById('OperationSelector');
const uploadFinishModal= document.getElementById('FinishModal');
const FinishModalText = document.getElementById("FinishModalText");
const WeightField = document.getElementById("WeightFieldtext");
const HeightField = document.getElementById("HeightFieldtext");

async function uploadScanData(inputElem=input, 
                                      checkElem=validationElem,
                                      opElem=OperationSelector,
                                      LineElem = LineSelector,
                                      WeightElem = WeightField,
                                      HeightElem = HeightField)
{
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
  if (LineElem.value=="0"){
    alert("select Line");
    return false;
  }
  if (opElem.value=="0"){
    alert("select Operation");
    return false;
  }
  const serverMainPath='';//'http://127.0.0.1:5001' ;
  payload = {
    serialNum:inputElem.value,
    LineNum:LineElem.value,
    OpStation: opElem.value,
    WeightNum: WeightElem.value,
    HeightNum: HeightElem.value,
    senderTag:'ManualWebFormUpload',
   // partCode: getPartCodeFromValidSerialNumber(inputElem.value),
  }
  response = await postDataStream(`${serverMainPath}/ManualScanData`, JSON.stringify(payload));
  uploadFinishModal.style.display='block';
  if (notifytheUpdate(response)){
    console.log('scan uploaded');
  };
  return true;
}

async function postDataStream(url, uploadData){ 
  let options={
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: uploadData,
    cache: 'default',
  }
  let response = await fetch(url, options); //`${serverMainPath}/partData`
  const data = await response.text();
  return data;
  }

function notifytheUpdate(text,elem=FinishModalText){
  if(text==null || text=='') {
    elem.innerText="Error : Data was not uploaded";
    console.log('did it run');
    return false;
  }
  elem.innerText=text;
  console.log(text);
  return true;
  }

function reloadWindow(){
  input.value='';
  location.reload();
}

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

//functions to populate the selector field
//function to get the lab stations configured in backend
async function getConfiguredLines(){
  let options={
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'text/plain',
    },
    body: "Get Lines in the control plan",
    cache: 'default',
  }
  const serverMainPath = "" ; 
  let response = await fetch(`${serverMainPath}/GetProductionLines`, options); //`${serverMainPath}/GetProductionLines`
  const data = await response.text();
  console.log(data);
  return data;
}

async function populateSelector(selElem, data){
  let JsonList = JSON.parse(data);
  selElem.innerHTML=""; //remove any previously added elements;
  selElem.appendChild(createElementForSelector("0","Select option"));
  JsonList.forEach(elem => {
    selElem.appendChild(createElementForSelector(elem,elem));
  });
}

function createElementForSelector(value,text){
  // ref element => <option value="0">Select Station</option>
  let elem = document.createElement("option");
  elem.setAttribute("value",value);
  elem.innerText=text;
  return elem;
}

async function operationsOfLine(event){
  let line = event.target.value; 
  let dat = await getOperationsOfLine(line);
  //console.log(dat); 
  //trial data//let dat ='["op1", "op2", "op3"]';
  populateSelector(OperationSelector,dat); //Global Variable used

}

async function getOperationsOfLine(Line){
  let options={
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'text/plain',
    },
    body: Line,
    cache: 'default',
  }
  const serverMainPath = "" ; 
  let response = await fetch(`${serverMainPath}/OperationInProdLine`, options); //`${serverMainPath}/GetProductionLines`
  const data = await response.text();
  return data;
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

