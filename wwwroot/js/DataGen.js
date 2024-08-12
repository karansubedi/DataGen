
const element = document.querySelector('.addButton');

const input = document.querySelector(".form-group");

const removebutton = document.querySelector('.removeButton');

const container = document.querySelector('.container');

const form = document.getElementById('dynamicForm');

const chatButton = document.getElementById('submitChat');

const chatReply = document.getElementById('chatReply');

const view1Button = document.getElementById('view1Button');

const view2Button = document.getElementById('view2Button');

const view3Button = document.getElementById('view3Button');

const view1 = document.getElementById('view1');

const view2 = document.getElementById('view2');

const view3 = document.getElementById('view3');


view1Button.addEventListener('click', function () {
    toggleView(view1, [view2, view3]);
    setActiveButton(view1Button, [view2Button, view3Button]);
});

view2Button.addEventListener('click', function () {
    toggleView(view2, [view1, view3]);
    setActiveButton(view2Button, [view1Button, view3Button]);
});


view3Button.addEventListener('click', function () {
    toggleView(view3, [view1, view2]);
    setActiveButton(view3Button, [view1Button, view2Button]);
});


chatButton.addEventListener('click', chatWithLLAMA);

element.addEventListener('click', function (event) {
    event.preventDefault();
    addInputField(event);
});

/*form.addEventListener('submit', function (event) {
    submitForm(event);
});*/

container.addEventListener('click', function (event) {

    if (event.target.classList.contains('removeButton')) {
        removefield(event);
    }
});


function submitForm(event) {
    event.preventDefault();

    const formData = new FormData(form);

    const serializedDataArray = [];

    const formGroups = document.querySelectorAll('.form-group');

    formGroups.forEach(group => {
        const formDataObject = {};

        group.querySelectorAll('input select').forEach(input => {
            const name = input.name;
            const value = input.value;

            if (formDataObject[name]) {
                if (!Array.isArray(formDataObject[name])) {
                    formDataObject[name] = [formDataObject[name]];
                }
                formDataObject[name].push(value);
            }
            else {
                formDataObject[name] = value;
            }
        });

        serializedDataArray.push(formDataObject);
    });

    

    const jsonString = JSON.stringify(serializedDataArray);

    console.log('Serialized form data array: ', jsonString);

    fetch('/DataGeneration/submitDynamicForm', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name = "__RequestVerificationToken"]').value
        },
        body: jsonString
    });
}

async function chatWithLLAMA(event) {

    event.preventDefault();

    var chatValue = document.getElementById('chat').value;

   const response = await fetch('/DataGeneration/chatWithLLAMA', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ chatValue: chatValue })
   });

        const data = await response.text();

        const paragraphText = document.createElement('p');

        paragraphText.textContent = data;

        chatReply.appendChild(paragraphText);
}

function populateDropdown() {
        var jsonData = [
            {
                "text": "Text",
                "value": "Text"
            },
            {
                "text": "DateTime",
                "value": "DateTime"
            },
            {
                "text": "Numeric",
                "value": "Numeric"
            }
        ];
        const options = jsonData;

        const dropdown = document.getElementById('dataTypeDropdownValue');

        dropdown.innerHTML = '';

        options.forEach(function (data) {
            var option = document.createElement('option');
            option.value = data.value;
            option.text = data.text;
            dropdown.appendChild(option);
        });

    /*});*/
}


function addInputField(event) {
    event.preventDefault();
    const input = document.querySelector('.container .form-group');
    var newFormGroup = document.createElement('div');
    newFormGroup.classList.add('form-group');
    newFormGroup.innerHTML = input.innerHTML;

    newFormGroup.querySelector('.removeButton').style.display = 'inline-block';

    container.appendChild(newFormGroup);
}


function removefield(event) {
    const formGroups = document.querySelectorAll('.form-group');

    event.target.parentElement.remove();
}

function toggleView(showView, hideViews) {
    showView.style.display = 'block';
    hideViews.forEach(function (view) {
        view.style.display = 'none';
    });
}

function setActiveButton(activeButton, inactiveButtons) {
    activeButton.classList.add('active');
    inactiveButtons.forEach(function (button) {
        button.classList.remove('active');
    });
}

window.onload = function () {
    populateDropdown();

    
};
