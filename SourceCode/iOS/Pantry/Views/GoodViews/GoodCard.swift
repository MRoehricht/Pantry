//
//  GoodCard.swift
//  Pantry
//
//  Created by Matthias Röhricht on 04.01.25.
//

import SwiftUI

struct GoodCard: View {
    var name: String
    
    @State private var isSelected = false
    var body: some View {
        VStack {
            HStack {
                Text(name)
                Spacer()
                Text("1kg")
            }.padding()
            
        }
        .onTapGesture { isSelected = true }
        .frame(maxWidth: .infinity)
        .clipShape(RoundedRectangle(cornerRadius: 10))
        .overlay(
            RoundedRectangle(cornerRadius: 10)
                .stroke(Color(red: 0.9, green: 0.9, blue: 0.9), lineWidth: 1)
                .shadow(radius: 1)
        )
        .padding([.top, .horizontal])
        .sheet(isPresented: $isSelected){
            VStack{
                HStack{
                    Text(name)
                    Spacer()
                    Text("1kg")
                }.padding()
                HStack{
                    Button {
                        print("Edit button was tapped")
                    } label: {
                        Label("mehr", systemImage: "chevron.up")
                            .padding()
                            .foregroundStyle(.white)
                            .background(.blue)
                    }
                    Button {
                        print("Edit button was tapped")
                    } label: {
                        Label("weniger", systemImage: "chevron.down")
                            .padding()
                            .foregroundStyle(.white)
                            .background(.red)
                    }
                }
                
                
            }
            .presentationDetents([.medium])
        }
    }
}

#Preview {
    GoodCard(name: "Cashew")
}
