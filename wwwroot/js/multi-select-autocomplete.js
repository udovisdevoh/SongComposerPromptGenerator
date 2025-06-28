// Fichier: wwwroot/js/multi-select-autocomplete.js

class MultiSelectAutocomplete {
    // Le constructeur accepte maintenant les valeurs initiales
    constructor(container, options, initialValues = []) {
        this.container = container;
        this.options = options;
        // On initialise l'ensemble des valeurs avec les valeurs initiales fournies
        this.selectedValues = new Set(initialValues);

        this.inputName = this.container.dataset.inputName;
        if (!this.inputName) {
            console.error("L'attribut 'data-input-name' est manquant sur le conteneur.", this.container);
            return;
        }

        this.initDOM();

        // On affiche immédiatement les "tags" et on crée les champs cachés correspondants
        this.renderTags();
        this.updateHiddenInputs();

        this.addEventListeners();
    }

    initDOM() {
        this.container.innerHTML = '';

        this.tagsContainer = document.createElement('div');
        this.tagsContainer.className = 'selected-tags';

        this.input = document.createElement('input');
        this.input.type = 'text';
        this.input.className = 'autocomplete-input form-control';
        this.input.placeholder = 'Cliquer ou rechercher...';

        this.suggestionsList = document.createElement('div');
        this.suggestionsList.className = 'suggestions-list';

        this.hiddenInputsContainer = document.createElement('div');
        this.hiddenInputsContainer.style.display = 'none';

        this.container.appendChild(this.tagsContainer);
        this.container.appendChild(this.input);
        this.container.appendChild(this.suggestionsList);
        this.container.appendChild(this.hiddenInputsContainer);
    }

    addEventListeners() {
        this.input.addEventListener('input', () => this.onInput());
        this.input.addEventListener('click', (e) => {
            e.stopPropagation();
            this.showAllSuggestions();
        });
        this.input.addEventListener('keydown', (e) => this.onKeyDown(e));
        document.addEventListener('click', () => {
            this.suggestionsList.style.display = 'none';
        });
    }

    onInput() {
        const value = this.input.value.trim().toLowerCase();
        if (value.length === 0) {
            this.suggestionsList.style.display = 'none';
            return;
        }
        const filteredOptions = this.options.filter(option =>
            option.toLowerCase().includes(value) && !this.selectedValues.has(option)
        );
        this.renderSuggestions(filteredOptions);
    }

    onKeyDown(event) {
        if (event.key === 'Enter') {
            event.preventDefault();
            const value = this.input.value.trim();
            if (value) {
                this.addValue(value);
                this.input.value = '';
                this.suggestionsList.style.display = 'none';
            }
        }
    }

    showAllSuggestions() {
        const availableOptions = this.options.filter(option => !this.selectedValues.has(option));
        this.renderSuggestions(availableOptions);
    }

    renderSuggestions(options) {
        this.suggestionsList.innerHTML = '';
        if (options.length === 0) {
            this.suggestionsList.style.display = 'none';
            return;
        }
        options.forEach(option => {
            const suggestionItem = document.createElement('div');
            suggestionItem.className = 'suggestion-item';
            suggestionItem.textContent = option;
            suggestionItem.addEventListener('click', () => {
                this.addValue(option);
                this.input.value = '';
                this.suggestionsList.style.display = 'none';
            });
            this.suggestionsList.appendChild(suggestionItem);
        });
        this.suggestionsList.style.display = 'block';
    }

    addValue(value) {
        if (this.selectedValues.has(value) || !value) return;
        this.selectedValues.add(value);
        this.renderTags();
        this.updateHiddenInputs();
    }

    removeValue(value) {
        this.selectedValues.delete(value);
        this.renderTags();
        this.updateHiddenInputs();
    }

    renderTags() {
        this.tagsContainer.innerHTML = '';
        this.selectedValues.forEach(value => {
            const tag = document.createElement('span');
            tag.className = 'tag';
            tag.textContent = value;
            const removeBtn = document.createElement('span');
            removeBtn.className = 'remove-tag';
            removeBtn.innerHTML = '&times;';
            removeBtn.addEventListener('click', () => this.removeValue(value));
            tag.appendChild(removeBtn);
            this.tagsContainer.appendChild(tag);
        });
    }

    updateHiddenInputs() {
        this.hiddenInputsContainer.innerHTML = '';
        this.selectedValues.forEach(value => {
            const hiddenInput = document.createElement('input');
            hiddenInput.type = 'hidden';
            hiddenInput.name = this.inputName;
            hiddenInput.value = value;
            this.hiddenInputsContainer.appendChild(hiddenInput);
        });
    }
}