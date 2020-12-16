import { Component, ElementRef, Input, OnInit, Self, ViewChild } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss']
})

// ControlValueAccessor - defines an interface that acts as a bridge between the angular forms API and the native element
// in the DOM
export class TextInputComponent implements OnInit, ControlValueAccessor {

  @ViewChild('input', { static: true }) input: ElementRef;
  @Input() type = 'text';
  @Input() label: string;

  // NgControl is what our form controls derive from
  // @Self() decorator is for angular dependency injection and angular is going to look for where to locate what
  // is going to inject into itself
  // if we have already activated service somewhere then application will woke up the tree of the dipendency hiearchy
  // looking for something that matches what we are injecting here

  // but if we use Self decorator here its only going to use this inside itself and not look for any
  // other shared dependency thats already in use
  constructor(@Self() public controlDir: NgControl) {
    this.controlDir.valueAccessor = this;
  }
    
  ngOnInit(): void {
    const control = this.controlDir.control;
    const validators = control.validator ? [control.validator] : [];
    const asyncValidators = control.asyncValidator ? [control.asyncValidator] : [];

    control.setValidators(validators);
    control.setAsyncValidators(asyncValidators);
    control.updateValueAndValidity();
  }

  onChange(event) { }

  onTouched() { }

  writeValue(obj: any): void {
    this.input.nativeElement.value = obj || '';
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
