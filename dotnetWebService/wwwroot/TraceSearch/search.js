const input = document.getElementById('serialInput');
const resultDisplay = document.getElementById('resultContainer');
const validationElem = document.getElementById('inputValidation');
validationElem.style.display="none";
resultDisplay.style.display="none";
const PORT=8080;
const serverMainPath=`http://127.0.0.1:${PORT}`;

//global data region
const resultContentMap = new Map();
const station = "processId"; //keys in json payload
const resultOP = 'operater';
const timeStamp = 'timeStamp';
const mach = 'machine';
const STAT = 'characteristicStatus';
const LAB  = 'labBlock';
const LABhidden  = "labHiddenBlock";
const stationDesc="opDesc";
resultContentMap.set(station,"operationStation"); //json key map to css attribute of the element
resultContentMap.set(resultOP,'operatorDetail');
resultContentMap.set(timeStamp, "timeStamp");
resultContentMap.set(mach, "machineDetail");
resultContentMap.set(STAT, getCharacteristicsStyling);
resultContentMap.set(LAB,'labLinkBox');
resultContentMap.set(LABhidden,'labLinkBoxHidden');
resultContentMap.set("stationBlock","resultOperationBlock")
resultContentMap.set(stationDesc,"OperationDesc");
const imgHeaderMap = new Map();
imgHeaderMap.set("operator", ["/img/operator.png", "operator imgSetter", "operator pic"]);
imgHeaderMap.set("scanner", ["/img/scanner.png", "scanner imgSetter" ,"scanner pic" ]);
imgHeaderMap.set("machine", ["/img/machine.jpg", "machine imgSetter", "machine pic" ]);
imgHeaderMap.set("Merkmal", ["/img/Merkmal.png", "merkmal imgSetter", "Characteristics"]);
//end global data region

//functions to run the input from the search bar 
input.addEventListener('input', triggerActionOnInput);
function triggerActionOnInput(e) {
    debugger;
  const value = e.target.value;
  if(value!="") 
  {
    const status = validateSerialNumber(value);
    showValidationMessage(validationElem,status);
    showDisplay(status,resultDisplay,1, value);
    return 1 ; 
  }
  showValidationMessage(validationElem,'hide');
  return 1;
}

function showDisplay(status, cntxt, type, value) {
    debugger
  if (status==true)
  {
    if (type==1) {
    cntxt.style.display="flex";
    generateResultHTML(cntxt,value);
    }
    if (type==2) {
      console.log('checked');
      cntxt.style.display="block";
      }
    return 1;
  }
  cntxt.style.display="none";
  return 0;
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

async function generateResultHTML(elem, serialData) {
    debugger
  const data = await getSerialData(serialData); 
  const temp = data.dataPoints;
  elem.innerHTML="";
  generateResultContainerInner(elem,temp);
  return 1;
}

async function getSerialData(serialNumber) {
    debugger
  let body = serialNumber; 
  let options={
    method: 'POST',
    headers: {
      Accept: 'application.json',
      'Content-Type': 'application/json'
    },
    body: body,
    cache: 'default'
  }
  let response = await fetch(`/partData`, options); //`${serverMainPath}/partData`
  const data = await response.json()
  //console.log(data);
  return data;
}

function generateResultContainerInner(elem, dataPoints){
  elem.appendChild(generateResultHeader());
  dataPoints.forEach(datapoint=>{
    elem.appendChild(generateResultBlock(datapoint));
  });
  //elem.appendChild(generateResultBlock({processId:'LabResult','labBlock':"serialNum",})); //test element of lab report block
  return 1;
}

function generateResultHeader(store=imgHeaderMap){
  const resultHeader = document.createElement("div");
  resultHeader.setAttribute("class","resultHeader");
  resultHeader.appendChild(generateresultHeaderTitle());
  store.forEach((value,key,map)=>{
    resultHeader.appendChild(generateImgContainer(key,map));
  })
  return resultHeader;
}

function generateresultHeaderTitle(){
  //<div class="resultHeaderTitle">Headers</div>
  const elem = document.createElement("div");
  elem.setAttribute("class","resultHeaderTitle");
  elem.innerText="Headers";
  return elem;
}
function generateImgContainer(type,store){
  //<div class="imgContainer"><imgItem/></div>
  const elem = document.createElement("div");
  elem.setAttribute("class","imgContainer");
  elem.appendChild(generateImgItem(type,store));
  return elem;
}
function generateImgItem(type, store){
  //<img  src="/img/operator.png" class="operator imgSetter" alt="operator pic" />
  const vals = store.get(type);
  const elem = document.createElement("img");
  elem.setAttribute("src",vals[0]);
  elem.setAttribute("class",vals[1]);
  elem.setAttribute("alt",vals[2]);
  return elem;
}

function generateResultBlock(resultObject){
  const elementBlock = document.createElement("div");
  elementBlock.setAttribute("class", "resultBlock");
  Object.keys(resultObject).filter(
    key=>key!='serialNum' && key!=stationDesc
    ).forEach((key,index,array) => {
    elementBlock.appendChild(generateResultItem(key,resultObject[key],resultObject));
  });
  if (resultObject.processId=='LabResult'){ //special addition to input hiddenElement for LabResult
    elementBlock.appendChild(generateResultItem(LABhidden,resultObject.labBlock,resultObject));
  }
  return elementBlock;
}

function generateResultItem(type, detail,resultObject){
  //<div class="resultItem resultItemContent"><resultContentElem/></div>
  const elemResultItem = document.createElement('div');
  elemResultItem.setAttribute("class", "resultItemContent");
  elemResultItem.appendChild(generateResultContent(type,detail,resultObject));
  return elemResultItem;
}

function generateResultContent(type,detail,resultObject,contentMap=resultContentMap){
  //<div class="operatorDetail">Operator1</div>
  const elem = document.createElement('div');
  if (type==STAT){
    elem.setAttribute("class", contentMap.get(type)(detail));
    elem.innerText=detail;
    return elem;
  }
  if(type==LAB){
    elem.setAttribute("class", contentMap.get(type));
    elem.appendChild(generateResultLabReportLink(type,detail));
    return elem;
  }
  if(type==station){
    //creating outerElement container for operationStaion and Operation Desc
    let parentElem=generateResultContent("stationBlock",null,resultObject,contentMap);
    //creating child elements OperationStation and Operation desc elements
    //childElem1 = operation Description elem
    if(!resultObject.hasOwnProperty(stationDesc)){
      resultObject[stationDesc]="";
    }
    let childElem1Text = resultObject[stationDesc].replaceAll(" ",'\u00a0'); //ref : https://stackoverflow.com/questions/10474124/replacing-spaces-with-nbsp
    parentElem.appendChild(generateResultContent(stationDesc,childElem1Text,resultObject,contentMap)); //creating operation description Element i.e childElem1 ;
    //childElem2 is operationStationElem
    let childElem2=elem; // 
    childElem2.setAttribute("class", contentMap.get(type));
    childElem2.innerText=detail;
    parentElem.appendChild(childElem2); 
    return childElem2; //correctionPending: currently non sending the whole operation Block , only sending the station element. return should be parentElem
  }

  elem.setAttribute("class", contentMap.get(type));
  elem.innerText=detail;
  return elem ;
}

function generateResultLabReportLink(type, serial) {
    debugger;
  const elem = document.createElement('a');
  elem.href=`/LabReportDisplay?serialNum=${serial}`;
  if(serial==null ) {
    elem.innerText="";
    return elem;  
  }
  //elem.innerText="Link";
  return elem;
}

function getCharacteristicsStyling(kind){
  if (kind.toLowerCase()=='ok'){
    return "statusSignal statusOk";
  }
  if (kind.toLowerCase()=='not-ok'){
    return "statusSignal statusNotOk";
  }
  return "statusSignal";
}

function validateSerialNumber(string) {
    debugger
   // const regexChecker = /^[0-9]{7};[0-9]{5};[1-3]{1};[a-zA-Z0-9]{5};[PG]{1}[0-9]{2};[a-zA-Z0-9]{5};[0-9]{2}:[0-9]{2}:[0-9]{2}$/i;
    const regexChecker = /^[a-zA-Z0-9]{9}$/;
  return regexChecker.test(string);
}

function testCaseForStringCheckers(){
  testCases= ["1234567;07365;3;AG672;G04;P4567;07:09:26",
              "1234567;07365;3;AG672;G04;P4567;07:09:26;",
              "1236789;11111;1;ABCDE;p01;A1234;22:03:06",
              "CA22L0015"];
  answers= [true,
            false,
          true];
  let val=0;
  testCases.every( (cas,index,arr) => {
    if (serialStringCheck(cas)==answers[index]){
      val=0;
      return true;
    }
    val =1 ;
    console.log("following case" , arr[index],"was wrongly identified as ", answers[index]);
    return false;
  })
  if (val==0) {
    console.log("All test string cases were appropriately categorized");
  }
  return 1;
}

//delete below lines , these are test lines
//testCaseForStringCheckers();
