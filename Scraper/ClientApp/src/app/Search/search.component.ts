import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { queryModel } from './query';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'search-component',
  templateUrl: './search.component.html'
})
export class SearchComponent {
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl

  }
  public baseUrl: string;
  public http: HttpClient;
  public result: response;

  model = new queryModel("","",10);
  searched = false;
  responseString = "";
  working = false;
  valid = true;
  onSubmit() {
    this.working = true;
    if (this.model.keyWords.length > 0 && this.model.query.length > 0) {
      this.http.post<response>(this.baseUrl + 'api/Scraper/scrape', this.model).subscribe(result => {
        this.result = result;
        this.searched = true;
        this.generateString();
      });
    }
    this.working = false;

  }
  getValid() {
    return this.valid;
  }
  getState() {
    return this.working;
  }
  generateString() {
    this.responseString ="'"+ this.result.query+"'" + " appeared at position(s) ";
    for (var i = 0; i < this.result.appeared.length; i++) {
      if (i + 1 == this.result.appeared.length) {
        this.responseString += this.result.appeared[i].toString() + ".";
      }
      else {
        this.responseString += this.result.appeared[i].toString() + ", ";
      }    
    }
  }
}

interface response {
  appeared: number[];
  requestedURL: string;
  keyWords: string;
  query: string;
}
