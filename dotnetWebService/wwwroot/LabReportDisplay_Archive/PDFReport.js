const PdfModal= document.getElementById('ReportViewModal');
const reportBox= document.getElementById("pdfBox");
const serialNum=new URLSearchParams(window.location.search).get("serialNum");
const pdfViewer =new URLSearchParams(window.location.search).get("pdfViewer");
const partIdHolder = document.getElementById('PartIdSpan');
const canvasElem=document.getElementById('pdfCanvas');
const TotalPagesElem=document.getElementById('page_count');
const PageNumCounter = document.getElementById('page_num');
pdfjsLib.GlobalWorkerOptions.workerSrc='./pdfJs/build/pdf.worker.js';
var PDFDoc=null;
var PageNum=1;
var PageRendering = false;
var PageNumPending = null;

function updatePartId(elem=partIdHolder,serial=serialNum){
  elem.innerText=serial;
  return true;
}

async function buildReportBlock(serial=serialNum, masterElem=reportBox)
{
  const serverMainPath= '';//'http://127.0.0.1:8080' ;//
  let payload = serial;
  response = await GetReportList(`${serverMainPath}/GetReportList`, payload);
  if (populatTheList(response,masterElem)){
    console.log('received report list');
    return true;
  };
  return false;
}

async function GetReportList(url, uploadData){ 
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
  //console.log(data);
  return data;
  }

async function fetchPdfData(url,identifier){ 
  let options={
    method: 'POST',
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
    },
    body: identifier, 
    cache: 'default',
  }
  let response = await fetch(url, options); //`${serverMainPath}/partData`
  const data = await response.text();
  return data;
  }

function populatTheList(text,elem){
  if(text==null || text=='') {
    elem.innerText="No reports are attached";
    return false;
  }
  const PdfList =JSON.parse(text);
  PdfList.forEach(element => {
    elem.appendChild(generateReportElem(element[1],element[0]));
  });
  return true;
  }

function generateReportElem(name,id){
  const elem=document.createElement('a');
  elem.setAttribute("class", "PdfLink");
  //elem.setAttribute("target","_blank");
  elem.setAttribute('reportID',id);
  elem.setAttribute('onclick','showReport(this);');
  elem.href='#';
  elem.innerText=name;
  return elem;
}

function reloadWindow(){
  location.reload();
}

function addReporttoBox(masterElem,report){
  masterElem.appendChild(generateReportElem(report.name,report.link));
  return masterElem;
}

async function showReport(elem) {
  PdfModal.style.display='block';
  displayPdf(elem.getAttribute('reportID'));
}

async function displayPdf(fileId,serial=serialNum){
  let reqData= {
    serial:serial,
    fileId:fileId
  }
  //console.log(reqData);
  const serverMainPath= "";//'http://127.0.0.1:8080' ; //
  let data = await fetchPdfData(`${serverMainPath}/GetReportData`,JSON.stringify(reqData));
  let passingData = atob(data);
  const loadingTask = pdfjsLib.getDocument({data:passingData});
  pdfCanvas(loadingTask);
  return true;
}

//function to display the pdf in the modal
const pdfCanvas = async (loadingTask)=>{
  PDFDoc=await loadingTask.promise;//global variable
  TotalPagesElem.textContent = PDFDoc.numPages;//global variable
   // Initial/first page rendering
   renderPage(PageNum);
};

async function renderPage(num){
  PageRendering=true; //global variable
  const page=await PDFDoc.getPage(num);
  const desiredWidth = canvasElem.width;//global variable
  const viewport=page.getViewport({scale:1});
  console.log(desiredWidth,viewport.width); //correctionPending: delete this line 
  let scale=1;//
  const scaledViewport=page.getViewport({scale:scale});
  const context=canvasElem.getContext('2d');
  const outputScale = window.devicePixelRatio || 1;
  canvasElem.width=Math.floor(scaledViewport.width*outputScale);//global variable
  canvasElem.style.width=Math.floor(scaledViewport.width)+"px";//global variable
  canvasElem.height=Math.floor(scaledViewport.height*outputScale);//global variable
  canvasElem.style.height=Math.floor(scaledViewport.height)+"px";//global variable
  //canvasElem.style.height=Math.floor(scaledViewport.width*outputScale);
  const transform = outputScale !== 1 
    ? [outputScale, 0, 0, outputScale, 0, 0] 
    : null;

  const renderContext={
      canvasContext:context,
      viewport:scaledViewport,
  };
  let renderTask = page.render(renderContext);
  await renderTask;
  PageRendering=false;//global variable
  if (PageNumPending!==null){
    renderPage(PageNumPending);
    PageNumPending = null;
  }
  // Update page counters
  PageNumCounter.textContent = num; //global variable
  return true;
}

/**
 * If another page rendering in progress, waits until the rendering is
 * finised. Otherwise, executes rendering immediately.
 */
 function queueRenderPage(num) {
  if (PageRendering) {
    PageNumPending = num;
  } else {
    renderPage(num);
  }
}

function onPrevPage() {
  if (PageNum <= 1) {
    return;
  }
  PageNum--;
  queueRenderPage(PageNum);
}
document.getElementById('prev').addEventListener('click', onPrevPage);

/**
 * Displays next page.
 */
function onNextPage() {
  if (PageNum >= PDFDoc.numPages) {
    return;
  }
  PageNum++;
  queueRenderPage(PageNum);
}
document.getElementById('next').addEventListener('click', onNextPage)

//reference PDF viewer understanding : 
//a. https://mozilla.github.io/pdf.js/examples/index.html#interactive-examples
//b. https://github.com/mozilla/pdf.js/blob/master/examples/learning/helloworld.html
//c. https://mozilla.github.io/pdf.js/getting_started/
